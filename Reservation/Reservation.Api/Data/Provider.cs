using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Reservation.Api.Models;

namespace Reservation.Api.Data
{
    public class Provider
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("created")]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        [Column("last_modified")]
        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        public static implicit operator Provider(ProviderDto dto)
        {
            return new Provider
            {
                Name = dto.Name,               
            };
        }

        public static implicit operator ProviderDto(Provider p)
        {
            return new ProviderDto(p.Name);
        }
    }
}