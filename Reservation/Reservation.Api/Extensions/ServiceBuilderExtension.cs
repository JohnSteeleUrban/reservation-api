using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Reservation.Api.Data;
using Reservation.Api.Services;

namespace Reservation.Api.Extensions
{
    public static class ServiceBuilderExtension
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers().AddNewtonsoftJson();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Reservations API",
                    Description = "A simple example ASP.NET 6 reservation api for clients and providers",
                    Contact = new OpenApiContact
                    {
                        Name = "John Steele Urban",
                        Email = "JohnSteeleUrban@gmail.com",
                        Url = new Uri("https://johnsteeleurban.com"),
                    }
                });
            });


            //add the db context to the di container
            services.AddDbContext<ReservationContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("ReservationsDb")));

            services.AddScoped<ReservationService>();

        }
    }
}
