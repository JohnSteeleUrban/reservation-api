using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reservation.Api.Models;
using Reservation.Api.Services;
using System.Net;

namespace Reservation.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly ReservationService _reservationService;
        private readonly ILogger _logger;

        public ClientsController(ReservationService service, ILogger<ClientsController> logger)
        {
            _reservationService = service;
            _logger = logger;

        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ClientDto client)
        {
            ActionResult result = StatusCode((int)HttpStatusCode.InternalServerError, "The content could not be displayed because an internal server error has occured.");

            try
            {
                var newAppointment = await _reservationService.AddClientAsync(client);

                //by design, returns null if validation fails and doesn't throw, 
                //or context fails to create without error.
                if (newAppointment == null)
                {
                    throw new InvalidOperationException($"Could not be created.");
                }

                result = StatusCode((int)HttpStatusCode.Created, newAppointment);
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