using Snacks_eCommerce.Models;
using Snacks_eCommerce.Services;
using Snacks_eCommerce.Validations;

namespace Snacks_eCommerce.Pages;

public partial class HomePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    public HomePage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _validator = validator;
        userName_label1.Text = "Hello, " + Preferences.Get("username", string.Empty);
        Title = AppConfig.PageTitle;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetCategoriesAsync();
        await GetBestSellerAsync();
        await GetPopularAsync();
    }

    private async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        try
        {
            var (categories, errorMessage) = await _apiService.GetCategoriesAsync();
            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Category>();
            }

            if (categories == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Could not find categories.", "OK");
                return Enumerable.Empty<Category>();
            }

            categories_collection.ItemsSource = categories;
            return categories;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not process request: {ex.Message}", "OK");
            return Enumerable.Empty<Category>();
        }
    }

    private async Task<IEnumerable<Product>> GetBestSellerAsync()
    {
        try
        {
            var (products, errorMessage) = await _apiService.GetProductsAsync("bestseller", string.Empty);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Product>();
            }

            if (products == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Could not find products.", "OK");
                return Enumerable.Empty<Product>();
            }

            bestSeller_collection.ItemsSource = products;
            return products;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not process request: {ex.Message}", "OK");
            return Enumerable.Empty<Product>();
        }
    }

    private async Task<IEnumerable<Product>> GetPopularAsync()
    {
        try
        {
            var (products, errorMessage) = await _apiService.GetProductsAsync("popular", string.Empty);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Product>();
            }

            if (products == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Could not find products.", "OK");
                return Enumerable.Empty<Product>();
            }
            popular_collection.ItemsSource = products;
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

    private void categories_collection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Category;
        if (currentSelection is null) return;

        Navigation.PushAsync(new CategoryProductsPage(currentSelection.Id, currentSelection.Name!,
            _apiService, _validator));

        ((CollectionView)sender).SelectedItem = null;
    }
}
