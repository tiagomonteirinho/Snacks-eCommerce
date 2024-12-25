using Microsoft.AspNetCore.Mvc; 
using FoodStore_WebApi.Entities;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;

    public ProductsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(string productType, int? categoryId = null)
    {
        IEnumerable<Product> products;

        if (productType == "category" && categoryId != null)
        {
            products = await _productRepository.GetCategoryProducts(categoryId.Value);
        }
        else if (productType == "popular")
        {
            products = await _productRepository.GetPopularProducts();
        }
        else if (productType == "bestseller")
        {
            products = await _productRepository.GetBestSellerProducts();
        }
        else
        {
            return BadRequest("Request could not be processed. Available productType field values: 'category' (with categoryId field value); 'popular'; 'bestseller'.");
        }

        var filteredProducts = products.Select(p => new
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            ImageUrl = p.ImageUrl
        });

        return Ok(filteredProducts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductDetails(int id)
    {
        var product = await _productRepository.GetProductDetails(id);
        if (product is null)
        {
            return NotFound($"That product could not be found.");
        }

        var productDetail = new
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Details = product.Details,
            ImageUrl = product.ImageUrl
        };

        return Ok(productDetail);
    }
}
