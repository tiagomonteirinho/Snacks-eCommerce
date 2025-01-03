﻿using Microsoft.Extensions.Logging;
using FoodStore_MauiApp.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FoodStore_MauiApp.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "https://q843l88r-7217.uks1.devtunnels.ms/";
    private readonly ILogger<ApiService> _logger;

    JsonSerializerOptions _serializerOptions;
    public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<ApiResponse<bool>> Register(string name, string email, string phoneNumber, string password)
    {
        try
        {
            var register = new Register()
            {
                Name = name,
                Email = email,
                PhoneNumber = phoneNumber,
                Password = password
            };

            var json = JsonSerializer.Serialize(register, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await PostRequest("api/Users/Register", content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Could not process HTTP request: {response.StatusCode}");
                return new ApiResponse<bool>
                {
                    ErrorMessage = $"Could not process HTTP request: {response.StatusCode}"
                };
            }

            return new ApiResponse<bool> { Data = true };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Could not register: {ex.Message}");
            return new ApiResponse<bool> { ErrorMessage = ex.Message };
        }
    }

    public async Task<ApiResponse<bool>> Login(string email, string password)
    {
        try
        {
            var login = new Login()
            {
                Email = email,
                Password = password
            };

            var json = JsonSerializer.Serialize(login, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await PostRequest("api/Users/Login", content);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Could not process HTTP request: {response.StatusCode}");
                return new ApiResponse<bool>
                {
                    ErrorMessage = $"Could not process HTTP request: {response.StatusCode}"
                };
            }

            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Token>(jsonResult, _serializerOptions);
            if (result == null)
            {
                _logger.LogError("Deserialization returned null.");
                return new ApiResponse<bool> { ErrorMessage = "Invalid response format." };
            }

            // Keep data in device, without having to generate it again.
            Preferences.Set("accesstoken", result!.AccessToken);
            Preferences.Set("userid", (int)result.UserId!);
            Preferences.Set("username", result.UserName);

            return new ApiResponse<bool> { Data = true };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Could not log in: {ex.Message}");
            return new ApiResponse<bool> { ErrorMessage = ex.Message };
        }
    }

    public async Task<ApiResponse<bool>> ConfirmOrder(Order order)
    {
        try
        {
            var json = JsonSerializer.Serialize(order, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await PostRequest("api/Orders", content);
            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = response.StatusCode == HttpStatusCode.Unauthorized ? "Unauthorized" : $"Could not process request: {response.ReasonPhrase}";
                _logger.LogError($"Could not process request: {response.StatusCode}");
                return new ApiResponse<bool> { ErrorMessage = errorMessage };
            }

            return new ApiResponse<bool> { Data = true };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Could not confirm order: {ex.Message}");
            return new ApiResponse<bool> { ErrorMessage = ex.Message };
        }
    }

    public async Task<ApiResponse<bool>> UploadUserImage(byte[] imageArray)
    {
        try
        {
            // MultipartFormDataContent: HTTP request form data content.
            var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(imageArray), "image", "image.jpg");
            var response = await PostRequest("api/users/UploadUserImage", content);
            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = response.StatusCode == HttpStatusCode.Unauthorized ? "Unauthorized" : $"Could not process request: {response.ReasonPhrase}";
                _logger.LogError($"Could not process request: {response.StatusCode}");
                return new ApiResponse<bool> { ErrorMessage = errorMessage };
            }

            return new ApiResponse<bool> { Data = true };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Could not upload image: {ex.Message}");
            return new ApiResponse<bool> { ErrorMessage = ex.Message };
        }
    }

    private async Task<HttpResponseMessage> PostRequest(string uri, HttpContent content)
    {
        var url = _baseUrl + uri;
        try
        {
            var result = await _httpClient.PostAsync(url, content);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Could not process HTTP request to {uri}: {ex.Message}");
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }

    public async Task<(List<Category>? categories, string? errorMessage)> GetCategories()
    {
        return await GetAsync<List<Category>>("api/categories");
    }

    public async Task<(List<Product>? products, string? errorMessage)> GetProducts(string productType, string categoryId)
    {
        string endpoint = $"api/products?productType={productType}&categoryId={categoryId}";
        return await GetAsync<List<Product>>(endpoint);
    }

    public async Task<(Product? productDetails, string? errorMessage)> GetProductDetails(int productId)
    {
        string endpoint = $"api/products/{productId}";
        return await GetAsync<Product>(endpoint);
    }

    public async Task<(List<ShoppingCartItem>? shoppingCartItems, string? errorMessage)> GetShoppingCartItems(int userId)
    {
        string endpoint = $"api/shoppingCartItems/{userId}";
        return await GetAsync<List<ShoppingCartItem>>(endpoint);
    }

    public async Task<(UserImage? userImage, string? errorMessage)> GetUserImage()
    {
        string endpoint = "api/users/userImage";
        return await GetAsync<UserImage>(endpoint);
    }

    public async Task<(List<UserOrder>? userOrders, string? errorMessage)> GetUserOrders(int userId)
    {
        string endpoint = $"api/orders/GetUserOrders/{userId}";
        return await GetAsync<List<UserOrder>>(endpoint);
    }

    public async Task<(List<OrderDetail>? orderDetails, string? errorMessage)> GetOrderDetails(int orderId)
    {
        string endpoint = $"api/orders/GetOrderDetails/{orderId}";
        return await GetAsync<List<OrderDetail>>(endpoint);
    }

    private async Task<(T? data, string? errorMessage)> GetAsync<T>(string endpoint)
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync(AppConfig.BaseUrl + endpoint);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<T>(responseString, _serializerOptions);

                // If data returns null, create generic type instance.
                return (data ?? Activator.CreateInstance<T>(), null);
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    string errorMessage = "Unauthorized";
                    _logger.LogWarning(errorMessage);
                    return (default, errorMessage);
                }

                string generalErrorMessage = $"Could not process HTTP request: {response.ReasonPhrase}";
                _logger.LogError(generalErrorMessage);
                return (default, generalErrorMessage);
            }
        }
        catch (HttpRequestException ex)
        {
            string errorMessage = $"Could not process HTTP request: {ex.Message}";
            _logger.LogError(ex, errorMessage);
            return (default, errorMessage);
        }
        catch (JsonException ex)
        {
            string errorMessage = $"Could not deserialize JSON: {ex.Message}";
            _logger.LogError(ex, errorMessage);
            return (default, errorMessage);
        }
        catch (Exception ex)
        {
            string errorMessage = $"Could not process request: {ex.Message}";
            _logger.LogError(ex, errorMessage);
            return (default, errorMessage);
        }
    }

    private void AddAuthorizationHeader()
    {
        var token = Preferences.Get("accesstoken", string.Empty);
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<ApiResponse<bool>> AddItemToShoppingCart(ShoppingCart shoppingCart)
    {
        try
        {
            var json = JsonSerializer.Serialize(shoppingCart, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await PostRequest("api/ShoppingCartItems", content);
            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = response.StatusCode == HttpStatusCode.Unauthorized ? "Unauthorized" : $"Could not process request: {response.ReasonPhrase}";
                _logger.LogError($"Could not process request: {response.StatusCode}");
                return new ApiResponse<bool> { ErrorMessage = errorMessage };
            }

            return new ApiResponse<bool> { Data = true };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Could not add item to cart: {ex.Message}");
            return new ApiResponse<bool> { ErrorMessage = ex.Message };
        }
    }

    public async Task<(bool data, string? errorMessage)> UpdateShoppingCartItemQuantity(int productId, string action)
    {
        try
        {
            var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
            var response = await PutRequest($"api/shoppingCartItems?productId={productId}&action={action}", content);
            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    string errorMessage = "Unauthorized";
                    _logger.LogWarning(errorMessage);
                    return (false, errorMessage);
                }

                string generalErrorMessage = $"Could not process HTTP request: {response.ReasonPhrase}";
                _logger.LogError(generalErrorMessage);
                return (false, generalErrorMessage);
            }
        }
        catch (HttpRequestException ex)
        {
            string errorMessage = $"Could not process HTTP request: {ex.Message}";
            _logger.LogError(ex, errorMessage);
            return (false, errorMessage);
        }
        catch (JsonException ex)
        {
            string errorMessage = $"Could not deserialize JSON: {ex.Message}";
            _logger.LogError(ex, errorMessage);
            return (false, errorMessage);
        }
        catch (Exception ex)
        {
            string errorMessage = $"Could not process request: {ex.Message}";
            _logger.LogError(ex, errorMessage);
            return (false, errorMessage);
        }
    }

    private async Task<HttpResponseMessage> PutRequest(string uri, HttpContent content)
    {
        var url = AppConfig.BaseUrl + uri;
        try
        {
            AddAuthorizationHeader();
            var result = await _httpClient.PutAsync(url, content);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Could not process HTTP request to {uri}: {ex.Message}");
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }
}
