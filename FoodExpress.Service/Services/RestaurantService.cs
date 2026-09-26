using FoodExpress.Service.Interfaces;
using FoodExpress.Domain.Interfaces.Repositories;
using FoodExpress.Domain.Entity;
namespace FoodExpress.Service.Services;

public class RestaurantService : IRestaurantService
{
    private readonly IRestaurantRepository _repository;

    public RestaurantService(
        IRestaurantRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Restaurant>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Restaurant?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task AddAsync(Restaurant restaurant)
    {
        if (restaurant == null)
            throw new ArgumentNullException(nameof(restaurant));

        await _repository.AddAsync(restaurant);
    }

    public async Task UpdateAsync(Restaurant restaurant)
    {
        if (restaurant == null)
            throw new ArgumentNullException(nameof(restaurant));

        var existing =
            await _repository.GetByIdAsync(restaurant.Id);

        if (existing == null)
            throw new KeyNotFoundException(
                "Restaurant was not found.");

        await _repository.UpdateAsync(restaurant);
    }

    public async Task DeleteAsync(int id)
    {
        var restaurant =
            await _repository.GetByIdAsync(id);

        if (restaurant == null)
            throw new KeyNotFoundException(
                "Restaurant was not found.");

        await _repository.DeleteAsync(id);
    }

    public async Task ActivateAsync(int id)
    {
        var restaurant =
            await _repository.GetByIdAsync(id);

        if (restaurant == null)
            throw new KeyNotFoundException(
                "Restaurant was not found.");

        restaurant.Activate();

        await _repository.UpdateAsync(restaurant);
    }

    public async Task DeactivateAsync(int id)
    {
        var restaurant =
            await _repository.GetByIdAsync(id);

        if (restaurant == null)
            throw new KeyNotFoundException(
                "Restaurant was not found.");

        restaurant.Deactivate();

        await _repository.UpdateAsync(restaurant);
    }
}