
using FoodExpress.Domain.Entity;
using FoodExpress.Domain.Interfaces;
using FoodExpress.Domain.Interfaces.Repositories;
using FoodExpress.Service.Interfaces;

namespace FoodExpress.Service.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICartService _cartService;
    private readonly IPromoCodeService _promoCodeService;
    private readonly IEmailService _emailService;
    private readonly ILogger _logger;

    public OrderService(
        IOrderRepository orderRepository,
        IUserRepository userRepository,
        ICartService cartService,
        IPromoCodeService promoCodeService,
        IEmailService emailService,
        ILogger logger)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _cartService = cartService;
        _promoCodeService = promoCodeService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Order> CreateOrderAsync(
        int userId,
        string address,
        decimal deliveryFee,
        string? promoCode = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException(
                    "Delivery address is required.");

            if (deliveryFee < 0)
                throw new ArgumentException(
                    "Delivery fee cannot be negative.");

            var user =
                await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException(
                    "User was not found.");

            var cart =
                await _cartService.GetCartAsync(userId);

            if (cart.IsEmpty())
                throw new InvalidOperationException(
                    "Cannot create order with empty cart.");

            var orderId =
                Guid.NewGuid().ToString("N");

            var order = new Order(
                orderId,
                userId,
                address,
                deliveryFee);

            foreach (var cartItem in cart.Items)
            {
                var orderItem = new OrderItem(
                    cartItem.Item.Id,
                    cartItem.Item.Name,
                    cartItem.Quantity,
                    cartItem.Item.GetFinalPrice());

                order.AddItem(orderItem);
            }

            if (!string.IsNullOrWhiteSpace(promoCode))
            {
                var discount =
                    await _promoCodeService
                        .CalculateDiscountAsync(
                            promoCode,
                            order.Subtotal);

                order.ApplyPromoCode(
                    promoCode,
                    discount);
            }

            await _orderRepository.AddAsync(order);

            await _cartService.ClearAsync(userId);

            await _logger.LogInfoAsync(
                $"Order created. OrderId: {order.Id}, " +
                $"UserId: {userId}, " +
                $"Total: {order.Total:F2}");

            await SendOrderConfirmationEmailAsync(
                user,
                order);

            return order;
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(
                $"Error creating order. UserId: {userId}",
                ex);

            throw;
        }
    }

    public async Task<Order?> GetByIdAsync(
        string orderId,
        int userId)
    {
        var order =
            await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
            return null;

        if (order.UserId != userId)
        {
            await _logger.LogErrorAsync(
                $"Unauthorized order access attempt. " +
                $"OrderId: {orderId}, UserId: {userId}");

            throw new UnauthorizedAccessException(
                "You cannot access another user's order.");
        }

        return order;
    }

    public async Task<List<Order>> GetMyOrdersAsync(
        int userId)
    {
        return await _orderRepository
            .GetByUserIdAsync(userId);
    }

    public async Task ConfirmAsync(
        string orderId,
        int userId)
    {
        try
        {
            var order =
                await GetByIdAsync(orderId, userId);

            if (order == null)
                throw new KeyNotFoundException(
                    "Order was not found.");

            order.Confirm();

            await _orderRepository.UpdateAsync(order);

            await _logger.LogInfoAsync(
                $"Order confirmed. " +
                $"OrderId: {order.Id}, UserId: {userId}");
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(
                $"Error confirming order. " +
                $"OrderId: {orderId}, UserId: {userId}",
                ex);

            throw;
        }
    }

    public async Task CancelAsync(
        string orderId,
        int userId)
    {
        try
        {
            var order =
                await GetByIdAsync(orderId, userId);

            if (order == null)
                throw new KeyNotFoundException(
                    "Order was not found.");

            order.Cancel();

            await _orderRepository.UpdateAsync(order);

            await _logger.LogInfoAsync(
                $"Order cancelled. " +
                $"OrderId: {order.Id}, UserId: {userId}");
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(
                $"Error cancelling order. " +
                $"OrderId: {orderId}, UserId: {userId}",
                ex);

            throw;
        }
    }

    // Admin methods

    public async Task<List<Order>> GetAllAsync()
    {
        return await _orderRepository.GetAllAsync();
    }

    public async Task<Order?> GetByIdForAdminAsync(
        string orderId)
    {
        return await _orderRepository
            .GetByIdAsync(orderId);
    }

    public async Task ConfirmForAdminAsync(
        string orderId)
    {
        try
        {
            var order =
                await GetByIdForAdminAsync(orderId);

            if (order == null)
                throw new KeyNotFoundException(
                    "Order was not found.");

            order.Confirm();

            await _orderRepository.UpdateAsync(order);

            await _logger.LogInfoAsync(
                $"Order confirmed by admin. " +
                $"OrderId: {order.Id}");
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(
                $"Error confirming order by admin. " +
                $"OrderId: {orderId}",
                ex);

            throw;
        }
    }

    public async Task StartPreparingAsync(
        string orderId)
    {
        try
        {
            var order =
                await GetByIdForAdminAsync(orderId);

            if (order == null)
                throw new KeyNotFoundException(
                    "Order was not found.");

            order.StartPreparing();

            await _orderRepository.UpdateAsync(order);

            await _logger.LogInfoAsync(
                $"Order preparation started. " +
                $"OrderId: {order.Id}");
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(
                $"Error starting preparation. " +
                $"OrderId: {orderId}",
                ex);

            throw;
        }
    }

    public async Task SendForDeliveryAsync(
        string orderId)
    {
        try
        {
            var order =
                await GetByIdForAdminAsync(orderId);

            if (order == null)
                throw new KeyNotFoundException(
                    "Order was not found.");

            order.SendForDelivery();

            await _orderRepository.UpdateAsync(order);

            await _logger.LogInfoAsync(
                $"Order sent for delivery. " +
                $"OrderId: {order.Id}");
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(
                $"Error sending order for delivery. " +
                $"OrderId: {orderId}",
                ex);

            throw;
        }
    }

    public async Task CompleteDeliveryAsync(
        string orderId)
    {
        try
        {
            var order =
                await GetByIdForAdminAsync(orderId);

            if (order == null)
                throw new KeyNotFoundException(
                    "Order was not found.");

            order.CompleteDelivery();

            await _orderRepository.UpdateAsync(order);

            await _logger.LogInfoAsync(
                $"Order delivered. " +
                $"OrderId: {order.Id}");
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(
                $"Error completing delivery. " +
                $"OrderId: {orderId}",
                ex);

            throw;
        }
    }

    public async Task CancelForAdminAsync(
        string orderId)
    {
        try
        {
            var order =
                await GetByIdForAdminAsync(orderId);

            if (order == null)
                throw new KeyNotFoundException(
                    "Order was not found.");

            order.Cancel();

            await _orderRepository.UpdateAsync(order);

            await _logger.LogInfoAsync(
                $"Order cancelled by admin. " +
                $"OrderId: {order.Id}");
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(
                $"Error cancelling order by admin. " +
                $"OrderId: {orderId}",
                ex);

            throw;
        }
    }

    private async Task SendOrderConfirmationEmailAsync(
        User user,
        Order order)
    {
        var body =
            BuildOrderEmailBody(order);

        var subject =
            $"FoodExpress - Order Confirmation #{order.Id}";

        try
        {
            await _emailService
                .SendOrderConfirmationAsync(
                    user.Email,
                    subject,
                    body);

            await _logger.LogInfoAsync(
                $"Order confirmation email sent. " +
                $"OrderId: {order.Id}, " +
                $"Email: {user.Email}");
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(
                $"Failed to send order confirmation email. " +
                $"OrderId: {order.Id}",
                ex);
        }
    }

    private static string BuildOrderEmailBody(
        Order order)
    {
        var lines = new List<string>();

        lines.Add("FoodExpress");
        lines.Add("Order Confirmation");
        lines.Add("----------------------------------------");
        lines.Add($"Order ID: {order.Id}");
        lines.Add(
            $"Date: {order.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        lines.Add(
            $"Delivery address: {order.Address}");
        lines.Add("");
        lines.Add("Items:");
        lines.Add("----------------------------------------");

        foreach (var item in order.Items)
        {
            lines.Add(
                $"{item.Name} x {item.Quantity} " +
                $"@ {item.UnitPrice:F2} ₾ = " +
                $"{item.LineTotal:F2} ₾");
        }

        lines.Add("----------------------------------------");
        lines.Add(
            $"Subtotal: {order.Subtotal:F2} ₾");
        lines.Add(
            $"Delivery fee: {order.DeliveryFee:F2} ₾");
        lines.Add(
            $"Discount: {order.Discount:F2} ₾");
        lines.Add(
            $"TOTAL: {order.Total:F2} ₾");
        lines.Add("");
        lines.Add($"Status: {order.Status}");
        lines.Add("");
        lines.Add("Thank you for using FoodExpress!");

        return string.Join(
            Environment.NewLine,
            lines);
    }
}
