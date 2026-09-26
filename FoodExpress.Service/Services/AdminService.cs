
using FoodExpress.Domain.Entity;
using FoodExpress.Service.Interfaces;

namespace FoodExpress.Service.Services;

public class AdminService : IAdminService
{
    private readonly IMenuService _menuService;
    private readonly IOrderService _orderService;
    private readonly IRestaurantService _restaurantService;

    public AdminService(
        IMenuService menuService,
        IOrderService orderService,
        IRestaurantService restaurantService)
    {
        _menuService = menuService;
        _orderService = orderService;
        _restaurantService = restaurantService;
    }

    public async Task AddMenuItemAsync(MenuItem item)
    {
        await _menuService.AddAsync(item);
    }

    public async Task UpdateMenuItemAsync(MenuItem item)
    {
        await _menuService.UpdateAsync(item);
    }

    public async Task DeleteMenuItemAsync(string itemId)
    {
        await _menuService.DeleteAsync(itemId);
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        return await _orderService.GetAllAsync();
    }

    public async Task<List<MenuItem>> GetMenuAsync()
    {
        return await _menuService.GetAllAsync();
    }

    public async Task<List<Restaurant>> GetRestaurantsAsync()
    {
        return await _restaurantService.GetAllAsync();
    }

    public async Task<Order?> GetOrderAsync(
        string orderId)
    {
        return await _orderService
            .GetByIdForAdminAsync(orderId);
    }

    public async Task ConfirmOrderAsync(
        string orderId)
    {
        await _orderService
            .ConfirmForAdminAsync(orderId);
    }

    public async Task StartPreparingAsync(
        string orderId)
    {
        await _orderService
            .StartPreparingAsync(orderId);
    }

    public async Task SendForDeliveryAsync(
        string orderId)
    {
        await _orderService
            .SendForDeliveryAsync(orderId);
    }

    public async Task CompleteDeliveryAsync(
        string orderId)
    {
        await _orderService
            .CompleteDeliveryAsync(orderId);
    }

    public async Task CancelOrderAsync(
        string orderId)
    {
        await _orderService
            .CancelForAdminAsync(orderId);
    }
}

