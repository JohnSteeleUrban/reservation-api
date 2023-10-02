using Newtonsoft.Json;

namespace Reservation.Api.Models
{
    public class ReservationDto
    {
        public ReservationDto(int providerId, int clientId, string title, DateTime time)
        {
            ProviderId = providerId;
            ClientId = clientId;
            Title = title;
            Time = time;
        }

        [JsonProperty(PropertyName = "providerId")]
        public int ProviderId { get; set; }

        [JsonProperty(PropertyName = "clientId")]
        public int ClientId { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "time")]
        public DateTime Time { get; set; }
    }
}
