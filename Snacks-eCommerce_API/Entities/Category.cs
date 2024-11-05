using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Snacks_eCommerce.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(99)]
        public string? Name { get; set; }

        [Required]
        [StringLength(99)]
        public string? ImageUrl { get; set; }

        [JsonIgnore]
        public ICollection<Product>? Products { get; set; }
    }
}
