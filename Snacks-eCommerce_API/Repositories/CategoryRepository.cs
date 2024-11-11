using Microsoft.EntityFrameworkCore;
using Snacks_eCommerce.Context;
using Snacks_eCommerce.Entities;

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