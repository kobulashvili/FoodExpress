using FoodExpress.Domain.Entity;
using FoodExpress.Domain.Interfaces.Repositories;
using FoodExpress.Infrastructure.Services;

namespace FoodExpress.Infrastructure.Repositories;

public class RestaurantRepository : IRestaurantRepository
{
    private readonly FileManager _fileManager;

    private const string FileName = "Restaurants.txt";

    public RestaurantRepository(FileManager fileManager)
    {
        _fileManager = fileManager;
    }

    public async Task<List<Restaurant>> GetAllAsync()
    {
        try
        {
            var lines = await _fileManager.ReadAllLinesAsync(FileName);
            var restaurants = new List<Restaurant>();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                try
                {
                    var restaurant = ParseRestaurant(line);

                    if (restaurant != null)
                        restaurants.Add(restaurant);
                }
                catch
                {
                    // Ignore invalid lines and continue.
                }
            }

            return restaurants;
        }
        catch (Exception ex)
        {
            throw new Exception(
                "Failed to load restaurants from Restaurants.txt.",
                ex);
        }
    }

    public async Task<Restaurant?> GetByIdAsync(int id)
    {
        var restaurants = await GetAllAsync();

        return restaurants.FirstOrDefault(
            x => x.Id == id);
    }

    public async Task AddAsync(Restaurant restaurant)
    {
        if (restaurant == null)
            throw new ArgumentNullException(nameof(restaurant));

        try
        {
            var restaurants = await GetAllAsync();

            if (restaurants.Any(x => x.Id == restaurant.Id))
            {
                throw new InvalidOperationException(
                    "A restaurant with this ID already exists.");
            }

            restaurants.Add(restaurant);

            await SaveAllAsync(restaurants);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception(
                "Failed to add restaurant.",
                ex);
        }
    }

    public async Task UpdateAsync(Restaurant restaurant)
    {
        if (restaurant == null)
            throw new ArgumentNullException(nameof(restaurant));

        try
        {
            var restaurants = await GetAllAsync();

            var index = restaurants.FindIndex(
                x => x.Id == restaurant.Id);

            if (index == -1)
                throw new KeyNotFoundException(
                    "Restaurant was not found.");

            restaurants[index] = restaurant;

            await SaveAllAsync(restaurants);
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception(
                "Failed to update restaurant.",
                ex);
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var restaurants = await GetAllAsync();

            var restaurant = restaurants.FirstOrDefault(
                x => x.Id == id);

            if (restaurant == null)
                throw new KeyNotFoundException(
                    "Restaurant was not found.");

            restaurants.Remove(restaurant);

            await SaveAllAsync(restaurants);
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception(
                "Failed to delete restaurant.",
                ex);
        }
    }

    private static Restaurant? ParseRestaurant(string line)
    {
        var parts = line.Split('|');

        if (parts.Length != 4)
            return null;

        var idText = parts[0].Trim();
        var name = parts[1].Trim();
        var address = parts[2].Trim();
        var isActiveText = parts[3].Trim();

        if (!int.TryParse(idText, out var id))
            return null;

        if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(address))
        {
            return null;
        }

        if (!bool.TryParse(
                isActiveText,
                out var isActive))
        {
            return null;
        }

        var restaurant = new Restaurant(
            id,
            name,
            address);

        if (!isActive)
            restaurant.Deactivate();

        return restaurant;
    }

    private async Task SaveAllAsync(
        List<Restaurant> restaurants)
    {
        var lines = restaurants.Select(
            FormatRestaurant);

        await _fileManager.WriteAllLinesAsync(
            FileName,
            lines);
    }

    private static string FormatRestaurant(
        Restaurant restaurant)
    {
        return string.Join(
            " | ",
            restaurant.Id,
            restaurant.Name,
            restaurant.Address,
            restaurant.IsActive);
    }
}