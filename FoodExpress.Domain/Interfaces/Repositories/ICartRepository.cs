using FoodExpress.Domain.Entity;

namespace FoodExpress.Domain.Interfaces.Repositories;

public interface ICartRepository
{
    Task<List<(string ItemId, int Quantity)>> GetItemsAsync(int userId);

    Task SaveAsync(
        int userId,
        Cart cart);

    Task DeleteAsync(int userId);
}