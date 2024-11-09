namespace Snacks_eCommerce.Pages;

public partial class AddressPage : ContentPage
{
    public AddressPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadSavedAddress();
    }

    private void LoadSavedAddress()
    {
        if (Preferences.ContainsKey("Name"))
            name_ent.Text = Preferences.Get("Name", string.Empty);
        if (Preferences.ContainsKey("PhoneNumber"))
            phoneNumber_ent.Text = Preferences.Get("PhoneNumber", string.Empty);
        if (Preferences.ContainsKey("Address"))
            address_ent.Text = Preferences.Get("Address", string.Empty);
    }

    private void save_btn_Clicked(object sender, EventArgs e)
    {
        Preferences.Set("Name", name_ent.Text);
        Preferences.Set("PhoneNumber", phoneNumber_ent.Text);
        Preferences.Set("Address", address_ent.Text);
        Navigation.PopAsync();
    }
}
