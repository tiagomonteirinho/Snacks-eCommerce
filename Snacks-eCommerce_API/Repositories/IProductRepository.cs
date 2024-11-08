using Snacks_eCommerce.Entities;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetBestSellerProducts();

    Task<IEnumerable<Product>> GetPopularProducts();

    Task<IEnumerable<Product>> GetCategoryProducts(int categoryId);

    Task<Product> GetProductDetails(int id);
}
