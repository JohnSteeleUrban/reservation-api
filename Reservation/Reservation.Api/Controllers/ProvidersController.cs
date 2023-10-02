using Microsoft.AspNetCore.Mvc;
using Reservation.Api.Models;
using Reservation.Api.Services;
using System.Net;

namespace Reservation.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProvidersController : ControllerBase
    {
        private readonly ReservationService _reservationService;
        private readonly ILogger _logger;

        public ProvidersController(ReservationService service, ILogger<ProvidersController> logger)
        {
            _reservationService = service;
            _logger = logger;

        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ProviderDto provider)
        {
            ActionResult result = StatusCode((int)HttpStatusCode.InternalServerError, "The content could not be displayed because an internal server error has occured.");

            try
            {
                var createdProvider = await _reservationService.AddProviderAsync(provider);

                //by design, returns null if validation fails and doesn't throw, 
                //or context fails to create without error.
                if (createdProvider == null)
                {
                    throw new InvalidOperationException($"Could not be created.");
                }

                result = StatusCode((int)HttpStatusCode.Created, createdProvider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }

            return result;
        }

        [HttpPost("Availability")]
        public async Task<IActionResult> CreateAvailabilityAsync([FromBody] AvailabilityDto availability)
        {
            ActionResult result = StatusCode((int)HttpStatusCode.InternalServerError, "The content could not be displayed because an internal server error has occured.");

            try
            {
                var createdAvailability = await _reservationService.AddAvailability(availability);

                //by design, returns null if validation fails and doesn't throw, 
                //or context fails to create without error.
                if (createdAvailability == null)
                {
                    throw new InvalidOperationException($"Could not be created.");
                }

                result = StatusCode((int)HttpStatusCode.Created, createdAvailability);
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
