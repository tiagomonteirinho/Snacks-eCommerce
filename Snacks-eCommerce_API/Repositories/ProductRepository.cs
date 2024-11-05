using Microsoft.EntityFrameworkCore;
using Snacks_eCommerce.Context;
using Snacks_eCommerce.Entities;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _appDbContext;

    public ProductRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IEnumerable<Product>> GetBestSellerProductsAsync()
    {
        return await _appDbContext.Products.Where(p => p.BestSeller).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetPopularProductsAsync()
    {
        return await _appDbContext.Products.Where(p => p.Popular).ToListAsync();
    }

    public async Task<Product> GetProductDetailAsync(int id)
    {
        var productDetail = await _appDbContext.Products.FirstOrDefaultAsync(p => p.Id == id);

        if (productDetail is null) 
            throw new InvalidOperationException("Product detail not found.");

        return productDetail!;
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _appDbContext.Products.Where(p => p.CategoryId == categoryId).ToListAsync();
    }
}