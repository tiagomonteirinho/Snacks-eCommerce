using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snacks_eCommerce.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(99)]
        public string? Name { get; set; }

        [Required]
        [StringLength(99)]
        public string? Email { get; set; }

        [Required]
        [StringLength(99)]
        public string? Password { get; set; }

        [StringLength(99)]
        public string? ImageUrl { get; set; }

        [StringLength(99)]
        public string? Phone { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }

        public ICollection<Order>? Orders { get; set; }
    }
}
