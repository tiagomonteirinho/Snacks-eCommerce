namespace FoodStore_MauiApp.Services;

public static class ServiceFactory
{
    // Create FavouritesService instance.
    public static FavouritesService CreateFavouritesService()
    {
        return new FavouritesService();
    }
}
