using FoodExpress.Domain.Entity;


namespace FoodExpress.Service.Interfaces;

public interface IMenuService
{
    Task<List<MenuItem>> GetAllAsync();

    Task<MenuItem?> GetByIdAsync(string id);

    Task<List<MenuItem>> FilterAsync(
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? category = null,
        int? restaurantId = null);

    Task AddAsync(MenuItem item);

    Task UpdateAsync(MenuItem item);

    Task DeleteAsync(string id);
}