using Microsoft.EntityFrameworkCore;
using FoodStore_WebApi.Context;
using FoodStore_WebApi.Entities;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _dbContext;

    public CategoryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Category>> GetCategories()
    {
        return await _dbContext.Categories.AsNoTracking().ToListAsync();
    }
}