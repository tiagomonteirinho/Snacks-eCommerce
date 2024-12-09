namespace Snacks_eCommerce.Models;

public class Product
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public decimal Price { get; set; }

    public string? Detail { get; set; }

    public string? ImageUrl { get; set; }

    public string? ImagePath => AppConfig.BaseUrl + ImageUrl;
}
