using Microsoft.EntityFrameworkCore;
using Reservation.Api.Data;
using Reservation.Api.Exceptions;
using Reservation.Api.Models;

namespace Reservation.Api.Services
{
    public class ReservationService
    {
        private readonly ReservationContext _reservationContext;


        public ReservationService(ReservationContext context)
        {
            _reservationContext = context;
        }

        public async Task<Provider> AddProviderAsync(ProviderDto provider)
        {
            var createdProvider = await _reservationContext.CreateAsync(provider);
            
            return createdProvider;
        }

        public async Task<Provider> GetProviderByIdAsync(int id)
        {
            var createdProvider = await _reservationContext.GetByIdAsync<Provider>(id);

            return createdProvider;
        }

        public async Task<Client> AddClientAsync(ClientDto provider)
        {
            var createdClient = await _reservationContext.CreateAsync<Client>(provider);

            return createdClient;
        }

        public async Task<Client> GetClientByIdAsync(int id)
        {
            var createdClient = await _reservationContext.GetByIdAsync<Client>(id);

            return createdClient;
        }


        #region reservation
        public async Task<bool> ConfirmReservation(int id)
        {
            using (var transaction = await _reservationContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var reservation = await _reservationContext.Reservations.FirstOrDefaultAsync(a => a.Id == id);

                    //if reservation does not exist or is not within 30 mins of creation it cannot be confirmed
                    if (reservation is null ||  (DateTime.UtcNow - reservation.Created).TotalMinutes > 30) return false;

                    reservation.IsConfirmed = true;
                    await _reservationContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    //log here
                    //if we were updating multiple entities this is especially helpful
                    transaction.Rollback();
                    throw new Exception("Unable to confirm");
                }
            }
            return true;
        }


        public async Task<Data.Reservation> AddReservationAsync(ReservationDto reservationDto)
        {
           await ValidateReservation(reservationDto);
            var reservation = await _reservationContext.CreateAsync<Data.Reservation>(reservationDto);

            return reservation;
        }

        private async Task<bool> ValidateReservation(ReservationDto reservationDto)
        {
            //reservations must be made 24 hours in advance
            if (reservationDto.Time < DateTime.UtcNow.AddDays(1)) throw new Exception("Reservations must be made 24 hours in advance.");

            // Make sure we are in 15 min increments (this is ugly but i can't think of a better solution in a short time frame.
            if (reservationDto.Time.Minute != 0 || 
                reservationDto.Time.Minute != 15 || 
                reservationDto.Time.Minute != 30 || 
                reservationDto.Time.Minute != 45)
                throw new Exception("Must schedule within 15 min intervals."); ;


            //check to make sure we are within an availability (provider shift) 
            var matchingAvailability = await _reservationContext.Availabilities
                                                            .FirstOrDefaultAsync(a =>
                                                                a.ProviderId == reservationDto.ProviderId &&
                                                                reservationDto.Time >= a.StartTime &&
                                                                reservationDto.Time < a.EndTime);

            if (matchingAvailability is null) 
                throw new Exception("No matching provider availability");            

            // check for overlapping resie
            var overlappingReservation = await _reservationContext.Reservations
                                                            .FirstOrDefaultAsync(r =>
                                                                r.ProviderId == reservationDto.ProviderId &&
                                                                r.Time == reservationDto.Time);

            if (overlappingReservation != null)
            {
                //we have an overlap
                if (overlappingReservation.IsConfirmed)
                {
                    throw new OverlappingTimeExeption("There is already a reservation during this time for your provider.");
                }
                
                //remove expired/unconfirmed reservation
                if(IsReservationExpired(overlappingReservation))
                {
                    _reservationContext.Reservations.Remove(overlappingReservation);
                }
            }                
        
            return true;
        }
        #endregion reservation


        #region availability
        public async Task<Availability?> AddAvailability(AvailabilityDto availability)
        {
            await ValidateAvailability(availability);

            var createdClient = await _reservationContext.CreateAsync<Availability>(availability);

            return createdClient;
        }

        private async Task<bool> ValidateAvailability(AvailabilityDto availability)
        {
            List<Availability?> availabilities = await GetAvailabilityByProviderId(availability.ProviderId);

            //if non exist, the new one is valid
            if (availabilities is null || !availabilities.Any()) return true;

            foreach (var existingAvailability in availabilities)
            {
                // Check if start/end times overlap with new request            
                if (availability.StartTime >= existingAvailability.StartTime && availability.StartTime < existingAvailability.EndTime)
                    throw new OverlappingTimeExeption($"Availability overlap, start: {existingAvailability.StartTime}, end: {existingAvailability.EndTime}");                 

                if (availability.EndTime > existingAvailability.StartTime && availability.EndTime <= existingAvailability.EndTime)                
                    throw new OverlappingTimeExeption($"Availability overlap. start: {existingAvailability.StartTime}, end: {existingAvailability.EndTime}");                

                if (availability.StartTime <= existingAvailability.StartTime && availability.EndTime >= existingAvailability.EndTime)                
                    throw new OverlappingTimeExeption($"Availability overlap. start: {existingAvailability.StartTime}, end: {existingAvailability.EndTime}");  
            }

            return true; 
        }

        public async Task<List<Availability?>> GetAvailabilityByProviderId(int id)
        {

            var result = await _reservationContext.Availabilities.Where(a => a.ProviderId == id).ToListAsync();
            return result;
        }

        public async Task<List<DateTime>> GetAvailableTimeSlotsAsync(int providerId, DateTime date)
        {
            //postgres is funny and requires this for the query to work.  it may mess with the exact times.
            var utcDate = date.ToUniversalTime();
            var availabilities = await _reservationContext.Availabilities.Where(a => a.ProviderId == providerId && a.StartTime.ToUniversalTime().Date == utcDate.Date).ToListAsync();


            var reservations = await _reservationContext.Reservations.Where(r => 
                                        (r.IsConfirmed || !r.IsConfirmed && (DateTime.UtcNow - r.Created).TotalMinutes > 30) && 
                                        r.ProviderId == providerId).ToListAsync();

            
            //get a list of all possible time slots and lets remove as we go
            var startTime = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            var endTime = new DateTime(date.Year, date.Month, date.Day, 23, 45, 0);
            var timeSlots = new List<DateTime>();

            for (var currentTime = startTime; currentTime <= endTime; currentTime = currentTime.AddMinutes(15))
            {
                timeSlots.Add(currentTime);
            }

            //this took a while and i haven't  thoroughly tested it.  It's probably waaaay more efficient to do something like this in a SPROC or
            //sql query syntax like i started in the comment below that way it can be optimized.
            //this entire method is probably super slow and who knows what kind of inefficient ugly sql is being
            //created under the hood.
               
            var availableTimeSlots = timeSlots.Where(timeSlot =>  
                                        {                                        
                                            bool hasAvailability = availabilities.Any(a => 
                                                                                        timeSlot >= a.StartTime && timeSlot < a.EndTime);
                                                        
                                            bool noReservation = !reservations.Any(r => 
                                                                                    timeSlot >= r.Time && timeSlot < r.Time.AddMinutes(15));
                                           
                                            return hasAvailability && noReservation;
                                        }
            ).ToList();

            return availableTimeSlots;

            //var res = await (from av _reservationContext.Availabilities
            //                 join res _reservationContext.Reservations on res.ProviderId equals av.ProviderId
            //                 where 
        }

        #endregion availability

        private bool IsReservationExpired(Data.Reservation reservation)
        {
            return !reservation.IsConfirmed && (DateTime.UtcNow - reservation.Created).TotalMinutes > 30;
        }
    }
}
