using FoodExpress.Domain.Entity;
using FoodExpress.Service.Interfaces;
using FoodExpress.UI.Helpers;

namespace FoodExpress.UI.UI;

public class CartUI
{
    private readonly ICartService _cartService;
    private readonly IMenuService _menuService;
    private readonly IOrderService _orderService;

    public CartUI(
        ICartService cartService,
        IMenuService menuService,
        IOrderService orderService)
    {
        _cartService = cartService;
        _menuService = menuService;
        _orderService = orderService;
    }

    public async Task ShowAsync(Customer customer)
    {
        while (true)
        {
            ConsoleHelper.ShowTitle("Cart");

            Console.WriteLine("1. View Cart");
            Console.WriteLine("2. Add Item");
            Console.WriteLine("3. Remove Item");
            Console.WriteLine("4. Change Quantity");
            Console.WriteLine("5. Clear Cart");
            Console.WriteLine("6. Checkout");
            Console.WriteLine("0. Back");
            Console.WriteLine();

            var choice =
                InputHelper.ReadString("Choose an option: ");

            try
            {
                switch (choice)
                {
                    case "1":
                        await DisplayAsync(customer.Id);
                        break;

                    case "2":
                        await AddItemAsync(customer.Id);
                        break;

                    case "3":
                        await RemoveItemAsync(customer.Id);
                        break;

                    case "4":
                        await ChangeQuantityAsync(
                            customer.Id);
                        break;

                    case "5":
                        await ClearAsync(customer.Id);
                        break;

                    case "6":
                        await CheckoutAsync(customer);
                        break;

                    case "0":
                        return;

                    default:
                        ConsoleHelper.ShowError(
                            "Invalid option.");

                        ConsoleHelper.Pause();
                        break;
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.ShowError(ex.Message);
                ConsoleHelper.Pause();
            }
        }
    }

    private async Task DisplayAsync(int userId)
    {
        var cart =
            await _cartService.GetCartAsync(userId);

        if (cart.IsEmpty())
        {
            Console.WriteLine("Cart is empty.");
            ConsoleHelper.Pause();
            return;
        }

        foreach (var item in cart.Items)
        {
            Console.WriteLine(
                $"ID: {item.Item.Id}");

            Console.WriteLine(
                $"Name: {item.Item.Name}");

            Console.WriteLine(
                $"Quantity: {item.Quantity}");

            Console.WriteLine(
                $"Unit price: " +
                $"{item.Item.GetFinalPrice():F2} ₾");

            Console.WriteLine(
                $"Line total: {item.LineTotal:F2} ₾");

            Console.WriteLine(
                "----------------------------------------");
        }

        Console.WriteLine(
            $"Total: {cart.TotalPrice:F2} ₾");

        ConsoleHelper.Pause();
    }

    private async Task AddItemAsync(int userId)
    {
        var itemId =
            InputHelper.ReadString("Item ID: ");

        var item =
            await _menuService.GetByIdAsync(itemId);

        if (item == null)
            throw new KeyNotFoundException(
                "Menu item was not found.");

        if (!item.IsAvailable)
            throw new InvalidOperationException(
                "This item is not available.");

        var quantity =
            InputHelper.ReadPositiveInt(
                "Quantity: ");

        await _cartService.AddItemAsync(
            userId,
            item,
            quantity);

        ConsoleHelper.ShowSuccess(
            "Item added to cart.");

        ConsoleHelper.Pause();
    }

    private async Task RemoveItemAsync(int userId)
    {
        var itemId =
            InputHelper.ReadString("Item ID: ");

        await _cartService.RemoveItemAsync(
            userId,
            itemId);

        ConsoleHelper.ShowSuccess(
            "Item removed from cart.");

        ConsoleHelper.Pause();
    }

    private async Task ChangeQuantityAsync(
        int userId)
    {
        var itemId =
            InputHelper.ReadString("Item ID: ");

        var quantity =
            InputHelper.ReadPositiveInt(
                "New quantity: ");

        await _cartService.ChangeQuantityAsync(
            userId,
            itemId,
            quantity);

        ConsoleHelper.ShowSuccess(
            "Quantity changed.");

        ConsoleHelper.Pause();
    }

    private async Task ClearAsync(int userId)
    {
        await _cartService.ClearAsync(userId);

        ConsoleHelper.ShowSuccess(
            "Cart cleared.");

        ConsoleHelper.Pause();
    }

    private async Task CheckoutAsync(
        Customer customer)
    {
        var cart =
            await _cartService.GetCartAsync(
                customer.Id);

        if (cart.IsEmpty())
            throw new InvalidOperationException(
                "Cannot checkout an empty cart.");

        Console.WriteLine(
            $"Cart total: {cart.TotalPrice:F2} ₾");

        var address =
            InputHelper.ReadString(
                $"Delivery address ({customer.Address}): ");

        var deliveryFee =
            InputHelper.ReadDecimal(
                "Delivery fee: ");

        if (deliveryFee < 0)
            throw new ArgumentException(
                "Delivery fee cannot be negative.");

        Console.Write(
            "Promo code (leave empty if none): ");

        var promoCode =
            Console.ReadLine();

        if (string.IsNullOrWhiteSpace(promoCode))
            promoCode = null;

        var order =
            await _orderService.CreateOrderAsync(
                customer.Id,
                address,
                deliveryFee,
                promoCode);

        ConsoleHelper.ShowSuccess(
            $"Order created successfully. " +
            $"Order ID: {order.Id}");

        Console.WriteLine(
            $"Total: {order.Total:F2} ₾");

        ConsoleHelper.Pause();
    }
}