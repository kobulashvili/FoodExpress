using FoodExpress.Domain.Enums;

namespace FoodExpress.Domain.Entity;

public class Order
{
    private readonly List<OrderItem> _items = new();

    public string Id { get; private set; }
    public int UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string Address { get; private set; }

    public decimal Subtotal =>
        _items.Sum(x => x.LineTotal);

    public decimal DeliveryFee { get; private set; }
    public decimal Discount { get; private set; }
    public string? PromoCode { get; private set; }
    public decimal Total =>
        Subtotal + DeliveryFee - Discount;

    public OrderStatus Status { get; private set; }

    public IReadOnlyCollection<OrderItem> Items =>
        _items.AsReadOnly();

    public Order(
        string id,
        int userId,
        string address,
        decimal deliveryFee)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException(
                "Address is required.");

        if (deliveryFee < 0)
            throw new ArgumentException(
                "Delivery fee cannot be negative.");

        Id = id;
        UserId = userId;
        Address = address;
        DeliveryFee = deliveryFee;
        CreatedAt = DateTime.Now;
        Status = OrderStatus.Created;
    }

    public void AddItem(OrderItem item)
    {
        if (Status != OrderStatus.Created)
            throw new InvalidOperationException(
                "Items cannot be added after order creation.");

        _items.Add(item);
    }

    public void ApplyDiscount(decimal discount)
    {
        if (discount < 0)
            throw new ArgumentException(
                "Discount cannot be negative.");

        if (discount > Subtotal + DeliveryFee)
            discount = Subtotal + DeliveryFee;

        Discount = discount;
    }


    public void ApplyPromoCode(
    string promoCode,
    decimal discount)
    {
        if (string.IsNullOrWhiteSpace(promoCode))
            throw new ArgumentException(
                "Promo code is required.");

        if (!string.IsNullOrWhiteSpace(PromoCode))
            throw new InvalidOperationException(
                "A promo code has already been applied.");

        if (discount < 0)
            throw new ArgumentException(
                "Discount cannot be negative.");

        PromoCode = promoCode;
        ApplyDiscount(discount);
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Created)
            throw new InvalidOperationException(
                "Order cannot be confirmed.");

        if (_items.Count == 0)
            throw new InvalidOperationException(
                "Cannot confirm an empty order.");

        Status = OrderStatus.Confirmed;
    }

    public void StartPreparing()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException(
                "Order must be confirmed first.");

        Status = OrderStatus.Preparing;
    }

    public void SendForDelivery()
    {
        if (Status != OrderStatus.Preparing)
            throw new InvalidOperationException(
                "Order must be preparing first.");

        Status = OrderStatus.OutForDelivery;
    }

    public void CompleteDelivery()
    {
        if (Status != OrderStatus.OutForDelivery)
            throw new InvalidOperationException(
                "Order must be out for delivery first.");

        Status = OrderStatus.Delivered;
    }

  
public void Cancel()
    {
        if (Status != OrderStatus.Created &&
            Status != OrderStatus.Confirmed)
        {
            throw new InvalidOperationException(
                "Order can only be cancelled when it is Created or Confirmed.");
        }

        Status = OrderStatus.Cancelled;
    }


    public static Order Rehydrate(
    string id,
    int userId,
    string address,
    decimal deliveryFee,
    decimal discount,
    string? promoCode,
    DateTime createdAt,
    OrderStatus status,
    List<OrderItem> items)
    {
        var order = new Order(
            id,
            userId,
            address,
            deliveryFee);

        order.CreatedAt = createdAt;
        order.Discount = discount;
        order.PromoCode = promoCode;
        order.Status = status;

        foreach (var item in items)
        {
            order._items.Add(item);
        }

        return order;
    }

}