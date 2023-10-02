using Newtonsoft.Json;

namespace Reservation.Api.Models
{
    public class ClientDto
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        public ClientDto(string name)
        {
            Name = name;
        }
    }
}
