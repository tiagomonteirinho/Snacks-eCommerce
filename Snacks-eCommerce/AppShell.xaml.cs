using Snacks_eCommerce.Pages;
using Snacks_eCommerce.Services;
using Snacks_eCommerce.Validations;

namespace Snacks_eCommerce
{
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
            var favouritesPage = new FavouritesPage();
            var accountPage = new AccountPage();

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
}
