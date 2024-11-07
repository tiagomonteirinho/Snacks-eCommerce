using Microsoft.AspNetCore.Mvc; 
using Snacks_eCommerce.Entities;

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
            products = await _productRepository.GetProductsByCategoryAsync(categoryId.Value);
        }
        else if (productType == "popular")
        {
            products = await _productRepository.GetPopularProductsAsync();
        }
        else if (productType == "bestseller")
        {
            products = await _productRepository.GetBestSellerProductsAsync();
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
    public async Task<IActionResult> GetProductDetail(int id)
    {
        var product = await _productRepository.GetProductDetailAsync(id);
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
