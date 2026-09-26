using FoodExpress.Domain.Entity;

namespace FoodExpress.Domain.Interfaces.Repositories;

public interface IMenuRepository
{
    Task<List<MenuItem>> GetAllAsync();

    Task<MenuItem?> GetByIdAsync(string id);

    Task AddAsync(MenuItem item);

    Task UpdateAsync(MenuItem item);

    Task DeleteAsync(string id);
}