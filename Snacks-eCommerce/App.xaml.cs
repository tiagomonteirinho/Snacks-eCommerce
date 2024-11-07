using Snacks_eCommerce.Pages;
using Snacks_eCommerce.Services;

namespace Snacks_eCommerce
{
    public partial class App : Application
    {
        private readonly ApiService _apiService;

        public App(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;

            MainPage = new NavigationPage(new RegisterPage(_apiService));
        }
    }
}
