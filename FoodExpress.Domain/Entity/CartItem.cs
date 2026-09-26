namespace FoodExpress.Domain.Entity;

public class CartItem
{
    public MenuItem Item { get; private set; }
    public int Quantity { get; private set; }

    public decimal LineTotal =>
        Item.GetFinalPrice() * Quantity;

    public CartItem(MenuItem item, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        Item = item;
        Quantity = quantity;
    }

    public void ChangeQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        Quantity = quantity;
    }
}