using Snacks_eCommerce.Models;
using Snacks_eCommerce.Services;
using Snacks_eCommerce.Validations;

namespace Snacks_eCommerce.Pages;

public partial class CategoryProductsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private int _categoryId;
    private bool _loginPageDisplayed = false;

    public CategoryProductsPage(int categoryId, string categoryName, ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _categoryId = categoryId;
        Title = categoryName ?? "Products";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetCategoryProducts(_categoryId);
    }

    private async Task<IEnumerable<Product>> GetCategoryProducts(int categoryId)
    {
        try
        {
            var (products, errorMessage) = await _apiService.GetProducts("category", categoryId.ToString());

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Product>();
            }

            if (products is null)
            {
                await DisplayAlert("Error", errorMessage ?? "Could not find products.", "OK");
                return Enumerable.Empty<Product>();
            }

            products_collection.ItemsSource = products;
            return products;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not process request: {ex.Message}", "OK");
            return Enumerable.Empty<Product>();
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private void products_collection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Product;
        if (currentSelection is null) return;

        Navigation.PushAsync(new ProductDetailsPage(currentSelection.Id, currentSelection.Name!,
            _apiService, _validator));

        ((CollectionView)sender).SelectedItem = null;
    }
}