using FoodExpress.Domain.Entity;

namespace FoodExpress.Service.Interfaces;

public interface ICartService
{
    Task<Cart> GetCartAsync(int userId);

    Task AddItemAsync(
        int userId,
        MenuItem item,
        int quantity);

    Task RemoveItemAsync(
        int userId,
        string itemId);

    Task ChangeQuantityAsync(
        int userId,
        string itemId,
        int quantity);

    Task ClearAsync(int userId);
}