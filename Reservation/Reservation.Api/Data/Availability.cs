using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Reservation.Api.Models;
using Newtonsoft.Json;

namespace Reservation.Api.Data
{
    public class Availability
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [ForeignKey("Provider")]
        public int ProviderId { get; set; }

        [JsonIgnore]
        public virtual Provider? Provider { get; set; }

        [Column("shift_start")]
        public DateTime StartTime { get; set; }

        [Column("shift_end")]
        public DateTime EndTime { get; set; }

        [Column("created")]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        [Column("last_modified")]
        public DateTime LastModified { get; set; } = DateTime.UtcNow;


        public static implicit operator Availability(AvailabilityDto dto)
        {
            return new Availability
            {
                ProviderId = dto.ProviderId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
            };
        }

        public static implicit operator AvailabilityDto(Availability p)
        {
            return new AvailabilityDto(p.ProviderId, p.StartTime, p.EndTime);
        }
    }
}