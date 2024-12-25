using FoodStore_MauiApp.Models;
using FoodStore_MauiApp.Services;
using FoodStore_MauiApp.Validations;

namespace FoodStore_MauiApp.Pages;

public partial class ProductDetailsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private int _productId;
    private bool _loginPageDisplayed = false;
    private FavouritesService _favouritesService = new FavouritesService();
    private string? _imageUrl;

    public ProductDetailsPage(int productId, string productName, ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _productId = productId;
        Title = productName ?? "Product";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetProductDetails(_productId);
        UpdateFavouriteButton();
    }

    private async Task<Product?> GetProductDetails(int productId)
    {
        var (product, errorMessage) = await _apiService.GetProductDetails(productId);

        if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
        {
            await DisplayLoginPage();
            return null;
        }

        if (product is null)
        {
            await DisplayAlert("Error", errorMessage ?? "Could not find product details.", "OK");
            return null;
        }

        if (product != null)
        {
            name_lbl.Text = product.Name;
            image.Source = product.ImagePath;
            unitPrice_lbl.Text = product.Price.ToString();
            description_lbl.Text = product.Detail;
            total_lbl.Text = product.Price.ToString();
            _imageUrl = product.ImagePath;
        }
        else
        {
            await DisplayAlert("Error", $"Could not process request: {errorMessage}", "OK");
            return null;
        }

        return product;
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private async void UpdateFavouriteButton()
    {
        var existingFavourite = await _favouritesService.ReadAsync(_productId);
        if (existingFavourite != null)
        {
            favourite_imgBtn.Source = "heartfill";
        }
        else
        {
            favourite_imgBtn.Source = "heart";
        }
    }

    private void decrement_btn_Clicked(object sender, EventArgs e)
    {
        if (int.TryParse(quantity_lbl.Text, out int quantity) && 
            decimal.TryParse(unitPrice_lbl.Text, out decimal unitPrice))
        {
            quantity = Math.Max(1, quantity - 1);
            quantity_lbl.Text = quantity.ToString();

            var total = quantity * unitPrice;
            total_lbl.Text = total.ToString();
        }
        else
        {
            DisplayAlert("Error", "Could not process request.", "OK");
        }
    }

    private void increment_btn_Clicked(object sender, EventArgs e)
    {
        if (int.TryParse(quantity_lbl.Text, out int quantity) &&
            decimal.TryParse(unitPrice_lbl.Text, out decimal unitPrice))
        {
            quantity++;
            quantity_lbl.Text = quantity.ToString();

            var total = quantity * unitPrice;
            total_lbl.Text = total.ToString();
        }
        else
        {
            DisplayAlert("Error", "Could not process request.", "OK");
        }
    }

    private async void addToCart_btn_Clicked(object sender, EventArgs e)
    {
        try
        {
            var shoppingCart = new ShoppingCart()
            {
                Quantity = Convert.ToInt32(quantity_lbl.Text),
                UnitPrice = Convert.ToDecimal(unitPrice_lbl.Text),
                Total = Convert.ToDecimal(total_lbl.Text),
                ProductId = _productId,
                UserId = Preferences.Get("userid", 0)
            };

            var response = await _apiService.AddItemToShoppingCart(shoppingCart);
            if (response.Data)
            {
                await DisplayAlert("Success", "Item successfully added to cart.", "OK");

                // Navigate to previous page.
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", $"Could not process request: {response.ErrorMessage}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not process request: {ex.Message}", "OK");
        }
    }

    private async void favourite_imgBtn_Clicked(object sender, EventArgs e)
    {
        try
        {
            var existingFavourite = await _favouritesService.ReadAsync(_productId);
            if (existingFavourite != null)
            {
                await _favouritesService.DeleteAsync(existingFavourite);
            }
            else
            {
                var favouriteProduct = new FavouriteProduct()
                {
                    ProductId = _productId,
                    Name = name_lbl.Text,
                    Price = Convert.ToDecimal(unitPrice_lbl.Text),
                    Detail = description_lbl.Text,
                    ImageUrl = _imageUrl,
                    IsFavourite = true,
                };

                await _favouritesService.CreateAsync(favouriteProduct);
            }

            UpdateFavouriteButton();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not process request: {ex.Message}", "OK");
        }
    }
}
