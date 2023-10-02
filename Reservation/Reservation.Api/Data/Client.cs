using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Reservation.Api.Models;

namespace Reservation.Api.Data
{
    public class Client
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

        public static implicit operator Client(ClientDto dto)
        {
            return new Client
            {
                Name = dto.Name,
            };
        }

        public static implicit operator ClientDto(Client p)
        {
            return new ClientDto(p.Name);
        }
    }
}