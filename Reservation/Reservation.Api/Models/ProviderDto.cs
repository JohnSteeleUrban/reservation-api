using Newtonsoft.Json;

namespace Reservation.Api.Models
{
    public class ProviderDto
    {
        public ProviderDto( string name)
        {
            Name = name;
        }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
