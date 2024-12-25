using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FoodStore_WebApi.Entities;

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
