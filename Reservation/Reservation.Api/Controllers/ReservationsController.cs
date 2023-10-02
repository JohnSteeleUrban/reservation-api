using Microsoft.AspNetCore.Mvc;
using Reservation.Api.Models;
using Reservation.Api.Services;
using System.Net;

namespace Reservation.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly ReservationService _reservationService;
        private readonly ILogger _logger;

        public ReservationsController(ReservationService service, ILogger<ReservationsController> logger)
        {
            _reservationService = service;
            _logger = logger;

        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ReservationDto reservationDto)
        {
            ActionResult result = StatusCode((int)HttpStatusCode.InternalServerError, "The content could not be displayed because an internal server error has occured.");

            try
            {
                var reservation = await _reservationService.AddReservationAsync(reservationDto);

                //by design, returns null if validation fails and doesn't throw, 
                //or context fails to create without error.
                if (reservation == null)
                {
                    throw new InvalidOperationException($"Could not be created.");
                }

                result = StatusCode((int)HttpStatusCode.Created, reservation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }

            return result;
        }

        [HttpPut("Confirm/{id}")]
        public async Task<IActionResult> Confirm(int id)
        {
            ActionResult result = StatusCode((int)HttpStatusCode.InternalServerError, "The content could not be displayed because an internal server error has occured.");

            try
            {
                bool confirmed = await _reservationService.ConfirmReservation(id);

                if (!confirmed)
                {
                    throw new InvalidOperationException($"Could not be confirmed.");
                }

                result = StatusCode((int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }

            return result;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DateTime>>> GetAvailableReservations(int providerId, DateTime date)
        {
            List<DateTime> result = new List<DateTime>();

            try
            {
                result = await _reservationService.GetAvailableTimeSlotsAsync(providerId, date);
                if (result == null)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }

            return result;
        }

    }
}
