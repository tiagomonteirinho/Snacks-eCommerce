namespace Snacks_eCommerce.Models;

public class UserImage
{
    public string? ImageUrl { get; set; }

    public string? ImagePath => AppConfig.BaseUrl + ImageUrl;
}
