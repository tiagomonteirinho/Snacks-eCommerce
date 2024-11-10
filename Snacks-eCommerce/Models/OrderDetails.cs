namespace Snacks_eCommerce.Models;

public class OrderDetail
{
    public int Id { get; set; }

    public int Quantity { get; set; }

    public decimal SubTotal { get; set; }

    public string? Name { get; set; }

    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public string ImagePath => AppConfig.BaseUrl + ImageUrl;
}