using FoodExpress.Domain.Entity;
using FoodExpress.Domain.Interfaces;
using FoodExpress.Domain.Interfaces.Repositories;
using FoodExpress.Service.Interfaces;
using System.Net;
using System.Net.Mail;
using System.Text;

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
        var itemsHtml = new StringBuilder();

        foreach (var item in order.Items)
        {
            itemsHtml.Append($@"
                <tr>
                    <td style='padding:12px; border-bottom:1px solid #eeeeee; color:#333333;'>
                        {item.Name}
                    </td>

                    <td style='padding:12px; border-bottom:1px solid #eeeeee; text-align:center; color:#555555;'>
                        {item.Quantity}
                    </td>

                    <td style='padding:12px; border-bottom:1px solid #eeeeee; text-align:right; color:#555555;'>
                        {item.UnitPrice:F2} ₾
                    </td>

                    <td style='padding:12px; border-bottom:1px solid #eeeeee; text-align:right; font-weight:bold; color:#333333;'>
                        {item.LineTotal:F2} ₾
                    </td>
                </tr>");
        }

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>FoodExpress Order</title>
</head>

<body style='margin:0; padding:0; background-color:#f4f6f8; font-family:Arial, Helvetica, sans-serif;'>

    <div style='max-width:650px; margin:30px auto; background:#ffffff; border-radius:12px; overflow:hidden;'>

        <div style='background:#ff6b35; padding:25px; text-align:center;'>
            <div style='font-size:32px; font-weight:bold; color:#ffffff;'>
                🍔 FoodExpress
            </div>

            <div style='margin-top:8px; color:#ffffff; font-size:15px;'>
                Order Confirmation
            </div>
        </div>

        <div style='padding:30px;'>

            <h2 style='margin-top:0; color:#333333;'>
                Thank you for your order!
            </h2>

            <p style='color:#555555; font-size:15px;'>
                Your order has been successfully created and is waiting for confirmation.
            </p>

            <div style='background:#f7f7f7; padding:18px; border-radius:8px; margin:25px 0;'>

                <p style='margin:0 0 8px 0; color:#777777; font-size:13px;'>
                    ORDER ID
                </p>

                <p style='margin:0; color:#222222; font-size:16px; font-weight:bold;'>
                    #{order.Id}
                </p>

                <p style='margin:15px 0 5px 0; color:#777777; font-size:13px;'>
                    DATE
                </p>

                <p style='margin:0; color:#333333; font-size:14px;'>
                    {order.CreatedAt:yyyy-MM-dd HH:mm:ss}
                </p>

                <p style='margin:15px 0 5px 0; color:#777777; font-size:13px;'>
                    DELIVERY ADDRESS
                </p>

                <p style='margin:0; color:#333333; font-size:14px;'>
                    {order.Address}
                </p>

            </div>

            <h3 style='color:#333333; margin-bottom:12px;'>
                Your Order
            </h3>

            <table style='width:100%; border-collapse:collapse; font-size:14px;'>

                <thead>
                    <tr style='background:#f7f7f7;'>
                        <th style='padding:12px; text-align:left; color:#555555;'>
                            Item
                        </th>

                        <th style='padding:12px; text-align:center; color:#555555;'>
                            Qty
                        </th>

                        <th style='padding:12px; text-align:right; color:#555555;'>
                            Price
                        </th>

                        <th style='padding:12px; text-align:right; color:#555555;'>
                            Total
                        </th>
                    </tr>
                </thead>

                <tbody>
                    {itemsHtml}
                </tbody>

            </table>

            <div style='margin-top:25px; background:#fafafa; padding:20px; border-radius:8px;'>

                <table style='width:100%; border-collapse:collapse;'>

                    <tr>
                        <td style='padding:6px 0; color:#666666;'>
                            Subtotal
                        </td>

                        <td style='padding:6px 0; text-align:right; color:#333333;'>
                            {order.Subtotal:F2} ₾
                        </td>
                    </tr>

                    <tr>
                        <td style='padding:6px 0; color:#666666;'>
                            Delivery fee
                        </td>

                        <td style='padding:6px 0; text-align:right; color:#333333;'>
                            {order.DeliveryFee:F2} ₾
                        </td>
                    </tr>

                    <tr>
                        <td style='padding:6px 0; color:#666666;'>
                            Discount
                        </td>

                        <td style='padding:6px 0; text-align:right; color:#2e8b57;'>
                            -{order.Discount:F2} ₾
                        </td>
                    </tr>

                    <tr>
                        <td style='padding-top:15px; border-top:2px solid #eeeeee; font-size:18px; font-weight:bold; color:#333333;'>
                            TOTAL
                        </td>

                        <td style='padding-top:15px; border-top:2px solid #eeeeee; text-align:right; font-size:20px; font-weight:bold; color:#ff6b35;'>
                            {order.Total:F2} ₾
                        </td>
                    </tr>

                </table>

            </div>

            <div style='margin-top:25px; text-align:center;'>

                <span style='display:inline-block; background:#fff3ed; color:#ff6b35; padding:10px 18px; border-radius:20px; font-size:14px; font-weight:bold;'>
                    Status: {order.Status}
                </span>

            </div>

            <p style='margin-top:30px; text-align:center; color:#777777; font-size:14px;'>
                Thank you for choosing FoodExpress! 🍔
            </p>

        </div>

        <div style='background:#333333; padding:20px; text-align:center;'>

            <p style='margin:0; color:#ffffff; font-size:13px;'>
                FoodExpress
            </p>

            <p style='margin:7px 0 0; color:#aaaaaa; font-size:11px;'>
                This is an automated email. Please do not reply.
            </p>

        </div>

    </div>

</body>
</html>";
    }
}
