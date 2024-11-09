using Snacks_eCommerce.Models;
using Snacks_eCommerce.Services;
using Snacks_eCommerce.Validations;
using System.Collections.ObjectModel;

namespace Snacks_eCommerce.Pages;

public partial class ShoppingCartPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    private ObservableCollection<ShoppingCartItem> ShoppingCartItems = new ObservableCollection<ShoppingCartItem>();

    public ShoppingCartPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetShoppingCartItems();
    }

    private async Task<IEnumerable<ShoppingCartItem>> GetShoppingCartItems()
    {
        try
        {
            var userId = Preferences.Get("userid", 0);
            var (shoppingCartItems, errorMessage) = await _apiService.GetShoppingCartItems(userId);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<ShoppingCartItem>();
            }

            if (shoppingCartItems is null)
            {
                await DisplayAlert("Error", errorMessage ?? "Could not find shopping cart items.", "OK");
                return Enumerable.Empty<ShoppingCartItem>();
            }

            ShoppingCartItems.Clear();
            foreach (var item in shoppingCartItems)
            {
                ShoppingCartItems.Add(item);
            }

            items_cv.ItemsSource = ShoppingCartItems;
            UpdateTotal();
            return shoppingCartItems;

        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not process request: {ex.Message}", "OK");
            return Enumerable.Empty<ShoppingCartItem>();
        }
    }

    private void UpdateTotal()
    {
        try
        {
            var total = ShoppingCartItems.Sum(item => item.Price * item.Quantity);
            total_lbl.Text = total.ToString();
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Could not process request: {ex.Message}", "OK");
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}
