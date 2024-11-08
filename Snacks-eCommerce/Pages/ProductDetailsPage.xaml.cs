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
            price_lbl.Text = $"{product.Price}€";
            description_lbl.Text = product.Detail;
            total_lbl.Text = $"{product.Price}€";
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
}