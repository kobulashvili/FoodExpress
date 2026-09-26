namespace FoodExpress.Domain.Entity;

public class OrderItem
{
    public string ItemId { get; private set; }
    public string Name { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    public decimal LineTotal =>
        Quantity * UnitPrice;

    public OrderItem(
        string itemId,
        string name,
        int quantity,
        decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        if (unitPrice <= 0)
            throw new ArgumentException(
                "Unit price must be greater than zero.");

        ItemId = itemId;
        Name = name;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}