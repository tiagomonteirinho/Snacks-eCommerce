﻿using Snacks_eCommerce.Pages;
using Snacks_eCommerce.Services;
using Snacks_eCommerce.Validations;

namespace Snacks_eCommerce;

public partial class App : Application
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    public App(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        SetMainPage();
    }

    private void SetMainPage()
    {
        // Get user token from preferences.
        var accessToken = Preferences.Get("accesstoken", string.Empty);

        if (string.IsNullOrEmpty(accessToken))
        {
            MainPage = new NavigationPage(new LoginPage(_apiService, _validator));
            return;
        }

        MainPage = new AppShell(_apiService, _validator);
    }
}
