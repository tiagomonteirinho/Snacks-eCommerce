using Snacks_eCommerce.Models;
using Snacks_eCommerce.Services;
using Snacks_eCommerce.Validations;

namespace Snacks_eCommerce.Pages;

public partial class OrdersPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    public OrdersPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetUserOrders();
    }

    private async Task GetUserOrders()
    {
        try
        {
            ordersLoaded_ai.IsRunning = true;
            ordersLoaded_ai.IsVisible = true;

            var (orders, errorMessage) = await _apiService.GetUserOrders(Preferences.Get("userid", 0));
            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return;
            }

            if (errorMessage == "NotFound")
            {
                await DisplayAlert("Error", "No existing orders.", "OK");
                return;
            }

            if (orders is null)
            {
                await DisplayAlert("Error", errorMessage ?? "Could not find user orders.", "OK");
                return;
            }
            else
            {
                orders_cv.ItemsSource = orders;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not process request: {ex.Message}", "OK");
        }
        finally
        {
            ordersLoaded_ai.IsRunning = false;
            ordersLoaded_ai.IsVisible = false;
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private void orders_cv_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as UserOrder;
        if (currentSelection == null) return;

        Navigation.PushAsync(new OrderDetailsPage(currentSelection.Id, currentSelection.Total,
            _apiService, _validator));

        ((CollectionView)sender).SelectedItem = null;
    }
}