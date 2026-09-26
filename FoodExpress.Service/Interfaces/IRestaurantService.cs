using FoodExpress.Domain.Entity;

namespace FoodExpress.Service.Interfaces;

public interface IRestaurantService
{
    Task<List<Restaurant>> GetAllAsync();

    Task<Restaurant?> GetByIdAsync(int id);

    Task AddAsync(Restaurant restaurant);

    Task UpdateAsync(Restaurant restaurant);

    Task DeleteAsync(int id);

    Task ActivateAsync(int id);

    Task DeactivateAsync(int id);
}