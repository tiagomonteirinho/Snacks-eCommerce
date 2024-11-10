using Snacks_eCommerce.Models;
using Snacks_eCommerce.Services;
using Snacks_eCommerce.Validations;

namespace Snacks_eCommerce.Pages;

public partial class FavouritesPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavouritesService _favouritesService;

    public FavouritesPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _favouritesService = new FavouritesService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetFavouriteProducts();
    }

    private async Task GetFavouriteProducts()
    {
        try
        {
            var favouriteProducts = await _favouritesService.ReadAllAsync();
            if (favouriteProducts is null || favouriteProducts.Count == 0)
            {
                favourites_cv.ItemsSource = null;
                empty_lbl.IsVisible = true;
            }
            else
            {
                favourites_cv.ItemsSource = favouriteProducts;
                empty_lbl.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not process request: {ex.Message}", "OK");
        }
    }

    private void favourites_cv_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as FavouriteProduct;
        if (currentSelection == null) return;

        Navigation.PushAsync(new ProductDetailsPage(currentSelection.ProductId, currentSelection.Name!,
            _apiService, _validator));

        ((CollectionView)sender).SelectedItem = null;
    }
}
