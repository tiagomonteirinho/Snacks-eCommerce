namespace Snacks_eCommerce.Pages;

public partial class EmptyShoppingCartPage : ContentPage
{
	public EmptyShoppingCartPage()
	{
		InitializeComponent();
	}

    private async void return_btn_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}