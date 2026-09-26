using FoodExpress.Domain.Entity;
using FoodExpress.Domain.Interfaces.Repositories;
using FoodExpress.Service.Interfaces;

namespace FoodExpress.Service.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IMenuService _menuService;

    public CartService(
        ICartRepository cartRepository,
        IMenuService menuService)
    {
        _cartRepository = cartRepository;
        _menuService = menuService;
    }

    public async Task<Cart> GetCartAsync(int userId)
    {
        var cart = new Cart();

        var items =
            await _cartRepository.GetItemsAsync(userId);

        foreach (var savedItem in items)
        {
            var menuItem =
                await _menuService.GetByIdAsync(
                    savedItem.ItemId);

            if (menuItem == null)
                continue;

            if (!menuItem.IsAvailable)
                continue;

            cart.AddItem(
                menuItem,
                savedItem.Quantity);
        }

        return cart;
    }

    public async Task AddItemAsync(
        int userId,
        MenuItem item,
        int quantity)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        if (!item.IsAvailable)
            throw new InvalidOperationException(
                "This item is not available.");

        var cart =
            await GetCartAsync(userId);

        cart.AddItem(
            item,
            quantity);

        await _cartRepository.SaveAsync(
            userId,
            cart);
    }

    public async Task RemoveItemAsync(
        int userId,
        string itemId)
    {
        var cart =
            await GetCartAsync(userId);

        cart.RemoveItem(itemId);

        await _cartRepository.SaveAsync(
            userId,
            cart);
    }

    public async Task ChangeQuantityAsync(
        int userId,
        string itemId,
        int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        var cart =
            await GetCartAsync(userId);

        cart.ChangeQuantity(
            itemId,
            quantity);

        await _cartRepository.SaveAsync(
            userId,
            cart);
    }

    public async Task ClearAsync(int userId)
    {
        await _cartRepository.DeleteAsync(userId);
    }
}