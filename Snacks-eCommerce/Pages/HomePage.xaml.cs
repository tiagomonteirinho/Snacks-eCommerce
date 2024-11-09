using Snacks_eCommerce.Models;
using Snacks_eCommerce.Services;
using Snacks_eCommerce.Validations;

namespace Snacks_eCommerce.Pages;

public partial class HomePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;
    private bool _isDataLoaded = false;

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

        // Execute tasks individually and await completion of each one before the next.
        //await GetCategories();
        //await GetBestSellerProducts();
        //await GetPopularProducts();

        if (!_isDataLoaded)
        {
            await LoadDataAsync();
            _isDataLoaded = true;
        }
    }

    private async Task LoadDataAsync()
    {
        // Execute tasks simultaneously and await completion of all.
        await Task.WhenAll(GetCategories(), GetBestSellerProducts(), GetPopularProducts());
    }

    private async Task<IEnumerable<Category>> GetCategories()
    {
        try
        {
            var (categories, errorMessage) = await _apiService.GetCategories();
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

    private async Task<IEnumerable<Product>> GetBestSellerProducts()
    {
        try
        {
            var (products, errorMessage) = await _apiService.GetProducts("bestseller", string.Empty);

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

            bestSellerProducts_collection.ItemsSource = products;
            return products;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not process request: {ex.Message}", "OK");
            return Enumerable.Empty<Product>();
        }
    }

    private async Task<IEnumerable<Product>> GetPopularProducts()
    {
        try
        {
            var (products, errorMessage) = await _apiService.GetProducts("popular", string.Empty);

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
            popularProducts_collection.ItemsSource = products;
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

    private void bestSellerProducts_collection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is CollectionView collectionView)
        {
            DisplayProductDetailsPage(collectionView, e);
        }
    }

    private void popularProducts_collection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is CollectionView collectionView)
        {
            DisplayProductDetailsPage(collectionView, e);
        }
    }

    private void DisplayProductDetailsPage(CollectionView collectionView, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Product;
        if (currentSelection is null) return;

        Navigation.PushAsync(new ProductDetailsPage(currentSelection.Id, currentSelection.Name!,
            _apiService, _validator));

        collectionView.SelectedItem = null;
    }
}
