using FoodExpress.Domain.Entity;

namespace FoodExpress.Domain.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync();

    Task<Order?> GetByIdAsync(string id);

    Task<List<Order>> GetByUserIdAsync(int userId);

    Task AddAsync(Order order);

    Task UpdateAsync(Order order);
}