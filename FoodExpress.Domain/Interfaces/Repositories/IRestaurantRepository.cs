using FoodExpress.Domain.Entity;

namespace FoodExpress.Domain.Interfaces.Repositories;

public interface IRestaurantRepository
{
    Task<List<Restaurant>> GetAllAsync();

    Task<Restaurant?> GetByIdAsync(int id);

    Task AddAsync(Restaurant restaurant);

    Task UpdateAsync(Restaurant restaurant);

    Task DeleteAsync(int id);
}