using FoodExpress.Domain.Entity;

namespace FoodExpress.Service.Interfaces;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(
        int userId,
        string address,
        decimal deliveryFee,
        string? promoCode = null);

    Task<Order?> GetByIdAsync(
        string orderId,
        int userId);

    Task<List<Order>> GetMyOrdersAsync(
        int userId);

    Task ConfirmAsync(
        string orderId,
        int userId);

    Task StartPreparingAsync(
        string orderId);

    Task SendForDeliveryAsync(
        string orderId);

    Task CompleteDeliveryAsync(
        string orderId);

    Task CancelAsync(
        string orderId,
        int userId);



    Task<List<Order>> GetAllAsync();
    Task<Order?> GetByIdForAdminAsync(string orderId);
    Task ConfirmForAdminAsync(string orderId);
    Task CancelForAdminAsync(string orderId);
}