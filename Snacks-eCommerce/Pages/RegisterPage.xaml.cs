using Snacks_eCommerce.Services;
using Snacks_eCommerce.Validations;

namespace Snacks_eCommerce.Pages;

public partial class RegisterPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    public RegisterPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    private async void signUp_button1_ClickedAsync(object sender, EventArgs e)
    {
        if (await _validator.Validate(name_entry1.Text, email_entry1.Text,
            phoneNumber_entry1.Text, password_entry1.Text))
        {
            var response = await _apiService.Register(name_entry1.Text, email_entry1.Text,
                phoneNumber_entry1.Text, password_entry1.Text);
            if (!response.HasError)
            {
                await DisplayAlert("Success", "Account successfully created.", "OK");
                await Navigation.PushAsync(new LoginPage(_apiService, _validator));
            }
            else
            {
                await DisplayAlert("Error", "Could not create account.", "OK");
            }
        }
        else
        {
            string errorMessage = "";
            errorMessage += _validator.NameError != null ? $"\n- {_validator.NameError}" : "";
            errorMessage += _validator.EmailError != null ? $"\n- {_validator.EmailError}" : "";
            errorMessage += _validator.PhoneNumberError != null ? $"\n- {_validator.PhoneNumberError}" : "";
            errorMessage += _validator.PasswordError != null ? $"\n- {_validator.PasswordError}" : "";
            await DisplayAlert("Error", errorMessage, "Ok");
        }
    }

    private async void logIn_tap1_TappedAsync(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}