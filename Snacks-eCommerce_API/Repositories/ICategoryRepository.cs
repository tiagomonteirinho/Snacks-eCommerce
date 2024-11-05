using Snacks_eCommerce.Entities;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetCategories();
}
