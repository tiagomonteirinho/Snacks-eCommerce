using FoodStore_MauiApp.Models;
using FoodStore_MauiApp.Services;
using FoodStore_MauiApp.Validations;
using System.Collections.ObjectModel;

namespace FoodStore_MauiApp.Pages;

public partial class ShoppingCartPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;
    private bool _isNavigatingToEmptyShoppingCartPage = false;

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

        if (IsNavigatingToEmptyShoppingCartPage()) return;

        bool hasItems = await GetShoppingCartItems();
        if (hasItems)
        {
            DisplayAddress();
        }
        else
        {
            await NavigateToEmptyShoppingCart();
        }
    }

    private void DisplayAddress()
    {
        bool savedAddress = Preferences.ContainsKey("Address");
        if (savedAddress)
        {
            string name = Preferences.Get("Name", string.Empty);
            string phone = Preferences.Get("PhoneNumber", string.Empty);
            string address = Preferences.Get("Address", string.Empty);
            address_lbl.Text = $"{name}\n{phone}\n{address}";
        }
        else
        {
            address_lbl.Text = "No address added.";
        }
    }

    private async Task NavigateToEmptyShoppingCart()
    {
        address_lbl.Text = string.Empty;
        _isNavigatingToEmptyShoppingCartPage = true;
        await Navigation.PushAsync(new EmptyShoppingCartPage());
    }

    private async Task<bool> GetShoppingCartItems()
    {
        try
        {
            var userId = Preferences.Get("userid", 0);
            var (shoppingCartItems, errorMessage) = await _apiService.GetShoppingCartItems(userId);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return false;
            }

            if (shoppingCartItems is null)
            {
                //await DisplayAlert("Error", errorMessage ?? "Could not find shopping cart items.", "OK");
                return false;
            }

            ShoppingCartItems.Clear();
            foreach (var item in shoppingCartItems)
            {
                ShoppingCartItems.Add(item);
            }

            items_cv.ItemsSource = ShoppingCartItems;
            UpdateTotal();

            if (!ShoppingCartItems.Any()) 
            { 
                return false; 
            }

            return true;

        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not process request: {ex.Message}", "OK");
            return false;
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

    private bool IsNavigatingToEmptyShoppingCartPage()
    {
        if (_isNavigatingToEmptyShoppingCartPage)
        {
            _isNavigatingToEmptyShoppingCartPage = false;
            return true;
        }

        return false;
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private async void decrement_btn_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is ShoppingCartItem shoppingCartItem)
        {
            if (shoppingCartItem.Quantity == 1) return;
            else
            {
                shoppingCartItem.Quantity--;
                UpdateTotal();
                await _apiService.UpdateShoppingCartItemQuantity(shoppingCartItem.ProductId, "decrease");
            }
        }
    }

    private async void increment_btn_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is ShoppingCartItem shoppingCartItem)
        {
            shoppingCartItem.Quantity++;
            UpdateTotal();
            await _apiService.UpdateShoppingCartItemQuantity(shoppingCartItem.ProductId, "increase");
        }
    }

    private async void remove_btn_Clicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is ShoppingCartItem shoppingCartItem)
        {
            bool response = await DisplayAlert("Confirm", "Do you wish to remove this item from the cart?", "Yes", "No");
            if (response)
            {
                ShoppingCartItems.Remove(shoppingCartItem);
                UpdateTotal();
                await _apiService.UpdateShoppingCartItemQuantity(shoppingCartItem.ProductId, "remove");
            }
        }
    }

    private void changeAddress_imgBtn_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new AddressPage());
    }

    private async void confirmOrder_tap_Tapped(object sender, TappedEventArgs e)
    {
        if (ShoppingCartItems is null || !ShoppingCartItems.Any())
        {
            await DisplayAlert("Error", "Could not find shopping cart items.", "OK");
            return;
        }

        var order = new Order()
        {
            Address = address_lbl.Text,
            Total = Convert.ToDecimal(total_lbl.Text),
            UserId = Preferences.Get("userid", 0)
        };

        var response = await _apiService.ConfirmOrder(order);
        if (response.HasError)
        {
            if (response.ErrorMessage == "Unauthorized")
            {
                await DisplayLoginPage();
                return;
            }

            await DisplayAlert("Error", $"Could not process request: {response.ErrorMessage}", "OK");
            return;
        }

        ShoppingCartItems.Clear();
        address_lbl.Text = "No address added.";
        total_lbl.Text = "0.00";
        await Navigation.PushAsync(new OrderConfirmedPage());
    }
}
