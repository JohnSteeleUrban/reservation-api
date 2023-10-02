using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Reservation.Api.Models;
using Newtonsoft.Json;

namespace Reservation.Api.Data
{
    [Table("Reservations")]
    public class Reservation
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [ForeignKey("Provider")]
        public int ProviderId { get; set; }

        [JsonIgnore]
        public virtual Provider? Provider { get; set; }

        [ForeignKey("Client")]
        public int ClientId { get; set; }

        [JsonIgnore]
        public virtual Client? Client { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("time")]
        public DateTime Time { get; set; }

        [Column("confirmed")]
        public bool IsConfirmed { get; set; } = false;

        [Column("created")]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        [Column("last_modified")]
        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        public static implicit operator Reservation(ReservationDto dto)
        {
            return new Reservation
            {
                ProviderId = dto.ProviderId,
                ClientId = dto.ClientId,
                Title = dto.Title,
                Time = dto.Time,
            };
        }

        public static implicit operator ReservationDto(Reservation r)
        {
            return new ReservationDto(r.ProviderId, r.ClientId, r.Title, r.Time);
        }
    }
}
