namespace FoodExpress.Domain.Enums;

public enum OrderStatus
{
    Created,
    Confirmed,
    Preparing,
    OutForDelivery,
    Delivered,
    Cancelled
}