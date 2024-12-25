using FoodStore_MauiApp.Services;

namespace FoodStore_MauiApp.Pages;

public partial class AccountSettingsPage : ContentPage
{
    private readonly ApiService _apiService;
    private const string UserNameKey = "username";
    private const string UserEmailKey = "email";
    private const string UserPhoneNumberKey = "phonenumber";

    public AccountSettingsPage(ApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        LoadAccountData();
        image_imgBtn.Source = await GetUserImage();
    }

    private void LoadAccountData()
    {
        name_lbl.Text = Preferences.Get(UserNameKey, string.Empty);
        name_ent.Text = Preferences.Get(UserNameKey, string.Empty);
        email_ent.Text = Preferences.Get(UserEmailKey, string.Empty);
        phoneNumber_ent.Text = Preferences.Get(UserPhoneNumberKey, string.Empty);
    }

    private async Task<string?> GetUserImage()
    {
        string defaultUserImageUrl = AppConfig.DefaultUserImageUrl;

        var (response, errorMessage) = await _apiService.GetUserImage();
        if (errorMessage != null)
        {
            switch (errorMessage)
            {
                case "Unauthorized":
                    await DisplayAlert("Error", "Could not process request.", "OK");
                    return defaultUserImageUrl;
                default:
                    await DisplayAlert("Error", errorMessage ?? "Could not find user image.", "OK");
                    return defaultUserImageUrl;
            }
        }

        if (response?.ImageUrl != null)
        {
            return response.ImagePath;
        }

        return defaultUserImageUrl;
    }

    private async void save_btn_Clicked(object sender, EventArgs e)
    {
        Preferences.Set(UserNameKey, name_ent.Text);
        Preferences.Set(UserEmailKey, email_ent.Text);
        Preferences.Set(UserPhoneNumberKey, phoneNumber_ent.Text);
        await DisplayAlert("Success", "Changes successfully saved.", "OK");
    }
}