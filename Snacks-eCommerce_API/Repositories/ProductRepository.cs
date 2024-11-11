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

    public async Task<IEnumerable<Product>> GetBestSellerProducts()
    {
        return await _appDbContext.Products.AsNoTracking().Where(p => p.BestSeller).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetPopularProducts()
    {
        return await _appDbContext.Products.AsNoTracking().Where(p => p.Popular).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetCategoryProducts(int categoryId)
    {
        return await _appDbContext.Products.AsNoTracking().Where(p => p.CategoryId == categoryId).ToListAsync();
    }

    public async Task<Product> GetProductDetails(int id)
    {
        var productDetail = await _appDbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

        if (productDetail is null) 
            throw new InvalidOperationException("Product detail not found.");

        return productDetail!;
    }
}