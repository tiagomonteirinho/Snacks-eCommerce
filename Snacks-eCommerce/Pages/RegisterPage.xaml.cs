using Snacks_eCommerce.Services;

namespace Snacks_eCommerce.Pages;

public partial class RegisterPage : ContentPage
{
    private readonly ApiService _apiService;

    public RegisterPage(ApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }

    private async void signUp_button1_ClickedAsync(object sender, EventArgs e)
    {
        var response = await _apiService.Register(name_entry1.Text, email_entry1.Text,
            phoneNumber_entry1.Text, password_entry1.Text);
        if (!response.HasError)
        {
            await DisplayAlert("Success", "Account successfully created.", "OK");
            await Navigation.PushAsync(new LoginPage(_apiService));
        }
        else
        {
            await DisplayAlert("Error", "Could not create account.", "OK");
        }
    }

    private async void logIn_tap1_TappedAsync(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new LoginPage(_apiService));
    }
}