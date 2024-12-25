using FoodStore_WebApi.Entities;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetCategories();
}
