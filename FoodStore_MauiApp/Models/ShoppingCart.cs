﻿namespace FoodStore_MauiApp.Models;

public class ShoppingCart
{
    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal Total { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }
}
