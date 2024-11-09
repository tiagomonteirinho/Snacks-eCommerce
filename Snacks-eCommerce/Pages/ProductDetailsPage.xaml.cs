using Snacks_eCommerce.Models;
using Snacks_eCommerce.Services;
using Snacks_eCommerce.Validations;

namespace Snacks_eCommerce.Pages;

public partial class ProductDetailsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private int _productId;
    private bool _loginPageDisplayed = false;

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

            var response = await _apiService.AddItemToCart(shoppingCart);
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
}
