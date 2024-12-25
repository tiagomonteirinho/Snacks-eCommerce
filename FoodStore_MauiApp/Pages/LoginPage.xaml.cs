using FoodStore_MauiApp.Services;
using FoodStore_MauiApp.Validations;

namespace FoodStore_MauiApp.Pages;

public partial class LoginPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    public LoginPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    private async void logIn_button1_ClickedAsync(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(email_entry1.Text))
        {
            await DisplayAlert("Error", "Email is required.", "OK");
            return;
        }

        if (string.IsNullOrEmpty(password_entry1.Text))
        {
            await DisplayAlert("Error", "Password is required.", "OK");
            return;
        }

        var response = await _apiService.Login(email_entry1.Text, password_entry1.Text);
        if (!response.HasError)
        {
            Application.Current!.MainPage = new AppShell(_apiService, _validator);
        }
        else
        {
            await DisplayAlert("Error", "Could not log in.", "OK");
        }
    }

    private async void signUp_tap1_TappedAsync(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage(_apiService, _validator));
    }
}