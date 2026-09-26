namespace FoodExpress.Domain.Entity;

public class Pizza : MenuItem
{
    public string Size { get; private set; }
    public string Dough { get; private set; }

    public Pizza(
        string id,
        int restaurantId,
        string name,
        string category,
        decimal price,
        string size,
        string dough)
        : base(id, restaurantId, name, category, price)
    {
        Size = size;
        Dough = dough;
    }

    public override decimal GetFinalPrice()
    {
        return Price;
    }

    public override string DisplayDetails()
    {
        return $"{Name} | Size: {Size} | Dough: {Dough} | Price: {Price:F2} ₾";
    }
}