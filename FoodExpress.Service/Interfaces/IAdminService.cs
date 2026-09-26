using FoodExpress.Domain.Entity;

namespace FoodExpress.Service.Interfaces;

public interface IAdminService
{
    Task AddMenuItemAsync(MenuItem item);

    Task UpdateMenuItemAsync(MenuItem item);

    Task DeleteMenuItemAsync(string itemId);

    Task<List<Order>> GetAllOrdersAsync();

    Task<List<MenuItem>> GetMenuAsync();

    Task<List<Restaurant>> GetRestaurantsAsync();

    Task<Order?> GetOrderAsync(string orderId);

    Task ConfirmOrderAsync(string orderId);

    Task StartPreparingAsync(string orderId);

    Task SendForDeliveryAsync(string orderId);

    Task CompleteDeliveryAsync(string orderId);

    Task CancelOrderAsync(string orderId);
}