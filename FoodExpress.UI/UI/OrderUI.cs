using FoodExpress.Domain.Entity;
using FoodExpress.Service.Interfaces;
using FoodExpress.UI.Helpers;

namespace FoodExpress.UI.UI;

public class OrderUI
{
    private readonly IOrderService _orderService;
    private readonly IAdminService _adminService;

    public OrderUI(
        IOrderService orderService,
        IAdminService adminService)
    {
        _orderService = orderService;
        _adminService = adminService;
    }

    public async Task ShowCustomerAsync(
        Customer customer)
    {
        while (true)
        {
            ConsoleHelper.ShowTitle(
                "My Orders");

            Console.WriteLine("1. View My Orders");
            Console.WriteLine("2. View Order");
            Console.WriteLine("3. Confirm Order");
            Console.WriteLine("4. Cancel Order");
            Console.WriteLine("0. Back");
            Console.WriteLine();

            var choice =
                InputHelper.ReadString("Choose an option: ");

            try
            {
                switch (choice)
                {
                    case "1":
                        await DisplayMyOrdersAsync(
                            customer.Id);
                        break;

                    case "2":
                        await DisplayMyOrderAsync(
                            customer.Id);
                        break;

                    case "3":
                        await ConfirmAsync(
                            customer.Id);
                        break;

                    case "4":
                        await CancelAsync(
                            customer.Id);
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

    public async Task ShowAdminAsync()
    {
        while (true)
        {
            ConsoleHelper.ShowTitle(
                "Order Management");

            Console.WriteLine("1. View All Orders");
            Console.WriteLine("2. View Order");
            Console.WriteLine("3. Confirm Order");
            Console.WriteLine("4. Start Preparing");
            Console.WriteLine("5. Send For Delivery");
            Console.WriteLine("6. Complete Delivery");
            Console.WriteLine("7. Cancel Order");
            Console.WriteLine("0. Back");
            Console.WriteLine();

            var choice =
                InputHelper.ReadString("Choose an option: ");

            try
            {
                switch (choice)
                {
                    case "1":
                        await DisplayAllOrdersAsync();
                        break;

                    case "2":
                        await DisplayAdminOrderAsync();
                        break;

                    case "3":
                        await ConfirmAdminAsync();
                        break;

                    case "4":
                        await StartPreparingAsync();
                        break;

                    case "5":
                        await SendForDeliveryAsync();
                        break;

                    case "6":
                        await CompleteDeliveryAsync();
                        break;

                    case "7":
                        await CancelAdminAsync();
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

    private async Task DisplayMyOrdersAsync(
        int userId)
    {
        var orders =
            await _orderService
                .GetMyOrdersAsync(userId);

        DisplayOrders(orders);

        ConsoleHelper.Pause();
    }

    private async Task DisplayMyOrderAsync(
        int userId)
    {
        var orderId =
            InputHelper.ReadString("Order ID: ");

        var order =
            await _orderService.GetByIdAsync(
                orderId,
                userId);

        if (order == null)
            throw new KeyNotFoundException(
                "Order was not found.");

        DisplayOrder(order);

        ConsoleHelper.Pause();
    }

    private async Task ConfirmAsync(int userId)
    {
        var orderId =
            InputHelper.ReadString("Order ID: ");

        await _orderService.ConfirmAsync(
            orderId,
            userId);

        ConsoleHelper.ShowSuccess(
            "Order confirmed.");

        ConsoleHelper.Pause();
    }

    private async Task CancelAsync(int userId)
    {
        var orderId =
            InputHelper.ReadString("Order ID: ");

        await _orderService.CancelAsync(
            orderId,
            userId);

        ConsoleHelper.ShowSuccess(
            "Order cancelled.");

        ConsoleHelper.Pause();
    }

    private async Task DisplayAllOrdersAsync()
    {
        var orders =
            await _adminService
                .GetAllOrdersAsync();

        DisplayOrders(orders);

        ConsoleHelper.Pause();
    }

    private async Task DisplayAdminOrderAsync()
    {
        var orderId =
            InputHelper.ReadString("Order ID: ");

        var order =
            await _adminService.GetOrderAsync(
                orderId);

        if (order == null)
            throw new KeyNotFoundException(
                "Order was not found.");

        DisplayOrder(order);

        ConsoleHelper.Pause();
    }

    private async Task ConfirmAdminAsync()
    {
        var orderId =
            InputHelper.ReadString("Order ID: ");

        await _adminService.ConfirmOrderAsync(
            orderId);

        ConsoleHelper.ShowSuccess(
            "Order confirmed.");

        ConsoleHelper.Pause();
    }

    private async Task StartPreparingAsync()
    {
        var orderId =
            InputHelper.ReadString("Order ID: ");

        await _adminService.StartPreparingAsync(
            orderId);

        ConsoleHelper.ShowSuccess(
            "Order preparation started.");

        ConsoleHelper.Pause();
    }

    private async Task SendForDeliveryAsync()
    {
        var orderId =
            InputHelper.ReadString("Order ID: ");

        await _adminService.SendForDeliveryAsync(
            orderId);

        ConsoleHelper.ShowSuccess(
            "Order sent for delivery.");

        ConsoleHelper.Pause();
    }

    private async Task CompleteDeliveryAsync()
    {
        var orderId =
            InputHelper.ReadString("Order ID: ");

        await _adminService.CompleteDeliveryAsync(
            orderId);

        ConsoleHelper.ShowSuccess(
            "Delivery completed.");

        ConsoleHelper.Pause();
    }

    private async Task CancelAdminAsync()
    {
        var orderId =
            InputHelper.ReadString("Order ID: ");

        await _adminService.CancelOrderAsync(
            orderId);

        ConsoleHelper.ShowSuccess(
            "Order cancelled.");

        ConsoleHelper.Pause();
    }

    private static void DisplayOrders(
        IEnumerable<Order> orders)
    {
        var list = orders.ToList();

        if (list.Count == 0)
        {
            Console.WriteLine("No orders found.");
            return;
        }

        foreach (var order in list)
        {
            Console.WriteLine(
                $"Order ID: {order.Id}");

            Console.WriteLine(
                $"User ID: {order.UserId}");

            Console.WriteLine(
                $"Date: {order.CreatedAt:yyyy-MM-dd HH:mm}");

            Console.WriteLine(
                $"Status: {order.Status}");

            Console.WriteLine(
                $"Total: {order.Total:F2} ₾");

            Console.WriteLine(
                "----------------------------------------");
        }
    }

    private static void DisplayOrder(Order order)
    {
        Console.WriteLine(
            $"Order ID: {order.Id}");

        Console.WriteLine(
            $"Date: {order.CreatedAt:yyyy-MM-dd HH:mm:ss}");

        Console.WriteLine(
            $"Address: {order.Address}");

        Console.WriteLine(
            $"Status: {order.Status}");

        Console.WriteLine();

        Console.WriteLine("Items:");

        foreach (var item in order.Items)
        {
            Console.WriteLine(
                $"{item.Name} x {item.Quantity} " +
                $"@ {item.UnitPrice:F2} ₾ = " +
                $"{item.LineTotal:F2} ₾");
        }

        Console.WriteLine();

        Console.WriteLine(
            $"Subtotal: {order.Subtotal:F2} ₾");

        Console.WriteLine(
            $"Delivery fee: {order.DeliveryFee:F2} ₾");

        Console.WriteLine(
            $"Discount: {order.Discount:F2} ₾");

        Console.WriteLine(
            $"Total: {order.Total:F2} ₾");
    }
}