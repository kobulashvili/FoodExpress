namespace FoodExpress.Domain.Entity;

public class Dessert : MenuItem
{
    public string PortionSize { get; private set; }

    public Dessert(
        string id,
        int restaurantId,
        string name,
        string category,
        decimal price,
        string portionSize)
        : base(id, restaurantId, name, category, price)
    {
        PortionSize = portionSize;
    }

    public override decimal GetFinalPrice()
    {
        return Price;
    }

    public override string DisplayDetails()
    {
        return $"{Name} | Portion: {PortionSize} | Price: {Price:F2} ₾";
    }
}