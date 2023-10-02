using Reservation.Api.Data;
using Reservation.Api.Extensions;

namespace Reservation.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.ConfigureAppConfiguration(configureDelegate: appConfiguration =>
                {
                    appConfiguration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    appConfiguration.AddJsonFile("appsettings.{env.EnvironmentName}.json", optional: true);
                });

            // Add services to the container.
            builder.Services.AddServices(builder.Configuration);
            
            var app = builder.Build();

#if DEBUG
            using var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<ReservationContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
#endif

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}