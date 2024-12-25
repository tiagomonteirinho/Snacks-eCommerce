namespace FoodStore_MauiApp.Pages;

public partial class OrderConfirmedPage : ContentPage
{
	public OrderConfirmedPage()
	{
		InitializeComponent();
	}

    private async void return_btn_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}