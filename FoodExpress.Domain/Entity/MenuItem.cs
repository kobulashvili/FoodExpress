namespace FoodExpress.Domain.Entity;

public abstract class MenuItem
{
    public string Id { get; private set; }
    public int RestaurantId { get; private set; }
    public string Name { get; private set; }
    public string Category { get; private set; }
    public decimal Price { get; private set; }
    public bool IsAvailable { get; private set; }

    protected MenuItem(
        string id,
        int restaurantId,
        string name,
        string category,
        decimal price)
    {
        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero.");

        Id = id;
        RestaurantId = restaurantId;
        Name = name;
        Category = category;
        Price = price;
        IsAvailable = true;
    }

    public void Enable()
    {
        IsAvailable = true;
    }

    public void Disable()
    {
        IsAvailable = false;
    }

    public abstract decimal GetFinalPrice();

    public abstract string DisplayDetails();
}