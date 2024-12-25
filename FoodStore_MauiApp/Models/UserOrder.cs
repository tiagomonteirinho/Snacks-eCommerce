namespace FoodStore_MauiApp.Models;

public class UserOrder
{
    public int Id { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal Total { get; set; }
}