using FoodStore_MauiApp.Services;
using FoodStore_MauiApp.Validations;

namespace FoodStore_MauiApp.Pages;

public partial class AccountPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    public AccountPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        name_lbl.Text = Preferences.Get("username", string.Empty);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        image_imgBtn.Source = await GetUserImage();
    }

    private async Task<string?> GetUserImage()
    {
        string defaultUserImageUrl = AppConfig.DefaultUserImageUrl;

        var (response, errorMessage) = await _apiService.GetUserImage();
        if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
        {
            await DisplayLoginPage();
            return null;
        }

        if (response == null)
        {
            await DisplayAlert("Error", errorMessage ?? "Could not find user image.", "OK");
            return defaultUserImageUrl;
        }

        if (response?.ImageUrl == null)
        {
            return defaultUserImageUrl;
        }

        return response.ImagePath;
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private async Task<byte[]?> ChangeUserImage()
    {
        try
        {
            // Allow image file upload. Required permissions at Platforms/Android/AndroidManifest.xml: CAMERA, READ_MEDIA_IMAGES and READ_MEDIA_VIDEO.
            var imageFile = await MediaPicker.PickPhotoAsync();
            if (imageFile is null) return null;

            using (var stream = await imageFile.OpenReadAsync())
            using (var memoryStream = new MemoryStream())
            {
                // Store image file stream in memory.
                await stream.CopyToAsync(memoryStream);

                // Convert memory stream to byte array.
                return memoryStream.ToArray();
            }
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("Error", "That feature is not supported on this platform.", "OK");
        }
        catch (PermissionException)
        {
            await DisplayAlert("Error", "That feature has not been granted required permissions.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not process request: {ex.Message}", "OK");
        }

        return null;
    }

    private async void image_imgBtn_Clicked(object sender, EventArgs e)
    {
        try
        {
            var imageArray = await ChangeUserImage();
            if (imageArray is null)
            {
                await DisplayAlert("Error", "Could not process request.", "OK");
                return;
            }

            image_imgBtn.Source = ImageSource.FromStream(() => new MemoryStream(imageArray));

            var response = await _apiService.UploadUserImage(imageArray);
            if (response.Data)
            {
                await DisplayAlert("Success", "Image successfully uploaded.", "OK");
            }
            else
            {
                await DisplayAlert("Error", response.ErrorMessage ?? "Could not process request.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not process request: {ex.Message}", "OK");
        }
    }

    private void orders_tap_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new OrdersPage(_apiService, _validator));
    }

    private void account_tap_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new AccountSettingsPage(_apiService));
    }

    private void faq_tap_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new FaqPage());
    }

    private void logout_imgBtn_Clicked(object sender, EventArgs e)
    {
        Preferences.Set("accesstoken", string.Empty);
        Application.Current!.MainPage = new NavigationPage(new LoginPage(_apiService, _validator));
    }
}