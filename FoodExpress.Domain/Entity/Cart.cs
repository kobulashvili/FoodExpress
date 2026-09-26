namespace FoodExpress.Domain.Entity;

public class Cart
{
    private readonly List<CartItem> _items = new();

    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    public decimal TotalPrice =>
        _items.Sum(x => x.LineTotal);

    public void AddItem(MenuItem item, int quantity)
    {
        if (!item.IsAvailable)
            throw new InvalidOperationException(
                "This item is not available.");

        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        var existingItem = _items
            .FirstOrDefault(x => x.Item.Id == item.Id);

        if (existingItem != null)
        {
            existingItem.ChangeQuantity(
                existingItem.Quantity + quantity);
        }
        else
        {
            _items.Add(new CartItem(item, quantity));
        }
    }

    public void RemoveItem(string itemId)
    {
        var item = _items
            .FirstOrDefault(x => x.Item.Id == itemId);

        if (item == null)
            throw new InvalidOperationException(
                "Item not found in cart.");

        _items.Remove(item);
    }

    public void ChangeQuantity(string itemId, int quantity)
    {
        var item = _items
            .FirstOrDefault(x => x.Item.Id == itemId);

        if (item == null)
            throw new InvalidOperationException(
                "Item not found in cart.");

        item.ChangeQuantity(quantity);
    }

    public void Clear()
    {
        _items.Clear();
    }

    public bool IsEmpty()
    {
        return _items.Count == 0;
    }
}