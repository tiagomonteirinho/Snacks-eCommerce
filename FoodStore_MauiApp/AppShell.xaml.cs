using FoodStore_MauiApp.Pages;
using FoodStore_MauiApp.Services;
using FoodStore_MauiApp.Validations;

namespace FoodStore_MauiApp;

public partial class AppShell : Shell
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    public AppShell(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        ConfigureShell();
    }

    private void ConfigureShell()
    {
        var homePage = new HomePage(_apiService, _validator);
        var shoppingCartPage = new ShoppingCartPage(_apiService, _validator);
        var favouritesPage = new FavouritesPage(_apiService, _validator);
        var accountPage = new AccountPage(_apiService, _validator);

        Items.Add(new TabBar
        {
            Items =
            {
                new ShellContent { Title = "Home", Icon = "home", Content = homePage },
                new ShellContent { Title = "Cart", Icon = "cart", Content = shoppingCartPage },
                new ShellContent { Title = "Favourites", Icon = "heart", Content = favouritesPage },
                new ShellContent { Title = "Account", Icon = "profile", Content = accountPage }
            }
        });
    }
}
