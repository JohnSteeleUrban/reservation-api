using Newtonsoft.Json;
using Reservation.Api.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reservation.Api.Models
{
    public class AvailabilityDto
    {
        public AvailabilityDto(int providerId, DateTime startTime, DateTime endTime)
        {
            ProviderId = providerId;
            StartTime = startTime;
            EndTime = endTime;
        }

        public int ProviderId { get; set; }

        [JsonProperty(PropertyName = "start_shift")]
        public DateTime StartTime { get; set; }

        [JsonProperty(PropertyName = "end_shift")]
        public DateTime EndTime { get; set; }
    }
}
