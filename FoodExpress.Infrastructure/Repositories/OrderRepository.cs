using System.Globalization;
using FoodExpress.Domain.Entity;
using FoodExpress.Domain.Enums;
using FoodExpress.Domain.Interfaces.Repositories;
using FoodExpress.Infrastructure.Services;

namespace FoodExpress.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly FileManager _fileManager;

    private const string OrdersFile = "Orders.txt";
    private const string OrderItemsFile = "OrderItems.txt";

    public OrderRepository(FileManager fileManager)
    {
        _fileManager = fileManager;
    }

    public async Task<List<Order>> GetAllAsync()
    {
        var orderLines =
            await _fileManager.ReadAllLinesAsync(OrdersFile);

        var itemLines =
            await _fileManager.ReadAllLinesAsync(OrderItemsFile);

        var orders = new List<Order>();

        foreach (var line in orderLines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split('|');

            if (parts.Length < 9)
                continue;

            if (!int.TryParse(parts[1].Trim(), out var userId))
                continue;

            if (!DateTime.TryParse(
                    parts[2].Trim(),
                    out var createdAt))
                continue;

            if (!decimal.TryParse(
                    parts[5].Trim(),
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out var deliveryFee))
                continue;

            if (!decimal.TryParse(
                    parts[6].Trim(),
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out var discount))
                continue;

            if (!Enum.TryParse<OrderStatus>(
                    parts[8].Trim(),
                    true,
                    out var status))
                continue;

            var orderId = parts[0].Trim();
            var address = parts[3].Trim();

            string? promoCode = null;

            if (parts.Length > 9 &&
                !string.IsNullOrWhiteSpace(parts[9].Trim()))
            {
                promoCode = parts[9].Trim();
            }

            var items = new List<OrderItem>();

            foreach (var itemLine in itemLines)
            {
                var itemParts = itemLine.Split('|');

                if (itemParts.Length != 6)
                    continue;

                if (!itemParts[0].Trim().Equals(
                        orderId,
                        StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!int.TryParse(
                        itemParts[3].Trim(),
                        out var quantity))
                    continue;

                if (!decimal.TryParse(
                        itemParts[4].Trim(),
                        NumberStyles.Number,
                        CultureInfo.InvariantCulture,
                        out var unitPrice))
                    continue;

                items.Add(
                    new OrderItem(
                        itemParts[1].Trim(),
                        itemParts[2].Trim(),
                        quantity,
                        unitPrice));
            }

            var order = Order.Rehydrate(
                orderId,
                userId,
                address,
                deliveryFee,
                discount,
                promoCode,
                createdAt,
                status,
                items);

            orders.Add(order);
        }

        return orders;
    }

    public async Task<Order?> GetByIdAsync(string id)
    {
        var orders = await GetAllAsync();

        return orders.FirstOrDefault(
            x => x.Id.Equals(
                id,
                StringComparison.OrdinalIgnoreCase));
    }

    public async Task<List<Order>> GetByUserIdAsync(int userId)
    {
        var orders = await GetAllAsync();

        return orders
            .Where(x => x.UserId == userId)
            .ToList();
    }

    public async Task AddAsync(Order order)
    {
        var orders = await GetAllAsync();

        if (orders.Any(x => x.Id == order.Id))
            throw new InvalidOperationException(
                "Order already exists.");

        orders.Add(order);

        await SaveAllAsync(orders);
    }

    public async Task UpdateAsync(Order order)
    {
        var orders = await GetAllAsync();

        var index = orders.FindIndex(
            x => x.Id == order.Id);

        if (index == -1)
            throw new KeyNotFoundException(
                "Order was not found.");

        orders[index] = order;

        await SaveAllAsync(orders);
    }

    private async Task SaveAllAsync(List<Order> orders)
    {
        var orderLines = orders.Select(order =>
            string.Join(
                " | ",
                order.Id,
                order.UserId,
                order.CreatedAt.ToString("O"),
                order.Address,
                order.Subtotal.ToString(
                    CultureInfo.InvariantCulture),
                order.DeliveryFee.ToString(
                    CultureInfo.InvariantCulture),
                order.Discount.ToString(
                    CultureInfo.InvariantCulture),
                order.Total.ToString(
                    CultureInfo.InvariantCulture),
                order.Status,
                order.PromoCode ?? ""));

        var itemLines = orders
            .SelectMany(order =>
                order.Items.Select(item =>
                    string.Join(
                        " | ",
                        order.Id,
                        item.ItemId,
                        item.Name,
                        item.Quantity,
                        item.UnitPrice.ToString(
                            CultureInfo.InvariantCulture),
                        item.LineTotal.ToString(
                            CultureInfo.InvariantCulture))));

        await _fileManager.WriteAllLinesAsync(
            OrdersFile,
            orderLines);

        await _fileManager.WriteAllLinesAsync(
            OrderItemsFile,
            itemLines);
    }
}