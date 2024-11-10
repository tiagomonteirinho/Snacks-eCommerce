using Snacks_eCommerce.Models;
using SQLite;

namespace Snacks_eCommerce.Services;

public class FavouritesService
{
    private readonly SQLiteAsyncConnection _database;

    public FavouritesService()
    {
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "favourites.db");
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<FavouriteProduct>().Wait();
    }

    public async Task<FavouriteProduct> ReadAsync(int id)
    {
        try
        {
            return await _database.Table<FavouriteProduct>().Where(p => p.ProductId == id).FirstOrDefaultAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<List<FavouriteProduct>> ReadAllAsync()
    {
        try
        {
            return await _database.Table<FavouriteProduct>().ToListAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task CreateAsync(FavouriteProduct FavouriteProduct)
    {
        try
        {
            await _database.InsertAsync(FavouriteProduct);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task DeleteAsync(FavouriteProduct FavouriteProduct)
    {
        try
        {
            await _database.DeleteAsync(FavouriteProduct);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
