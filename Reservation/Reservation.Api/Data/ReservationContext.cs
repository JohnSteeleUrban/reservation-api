using Microsoft.EntityFrameworkCore;
using Reservation.Api.Models;
using System;

namespace Reservation.Api.Data
{
    public class ReservationContext : DbContext
    {
        public ReservationContext(DbContextOptions<ReservationContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //PostgreSQL has a default schema of public
            //otherwise user dbo for ms sql server
            modelBuilder.HasDefaultSchema("public");
#if DEBUG
            #region Seed
            DateTime dateTime = DateTime.UtcNow.AddDays(1).AddHours(1);
            modelBuilder.Entity<Provider>().HasData(new Provider { Id = 1, Name = "Dr. Jekyll",  });
            modelBuilder.Entity<Client>().HasData(new Client { Id = 1, Name = "Jeff Lebowski", });
            modelBuilder.Entity<Availability>().HasData(new Availability { Id = 1, StartTime = DateTime.UtcNow.AddDays(1), EndTime = DateTime.UtcNow.AddDays(1).AddHours(8), ProviderId = 1 });
            //modelBuilder.Entity<Reservation>().HasData(new Reservation { Id = 1, Title = "new doc appt", ClientId = 1, ProviderId = 1, Time = DateTime.UtcNow.Date. new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0)});     
            #endregion
#endif
            base.OnModelCreating(modelBuilder);
        }

        public async Task<TEntity> CreateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            await this.Set<TEntity>().AddAsync(entity);
            await this.SaveChangesAsync();
            return entity;
        }

        public async Task<TEntity> GetByIdAsync<TEntity>(int id) where TEntity : class
        {
            return await this.Set<TEntity>().FindAsync(id);
        }

        //declare entities

        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Provider> Providers { get; set; }       
    }
}
