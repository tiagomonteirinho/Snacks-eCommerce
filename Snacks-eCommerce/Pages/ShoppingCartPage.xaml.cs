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
}
