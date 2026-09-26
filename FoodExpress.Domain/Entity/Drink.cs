namespace FoodExpress.Domain.Entity;

public class Drink : MenuItem
{
    public string Volume { get; private set; }

    public Drink(
        string id,
        int restaurantId,
        string name,
        string category,
        decimal price,
        string volume)
        : base(id, restaurantId, name, category, price)
    {
        Volume = volume;
    }

    public override decimal GetFinalPrice()
    {
        return Price;
    }

    public override string DisplayDetails()
    {
        return $"{Name} | Volume: {Volume} | Price: {Price:F2} ₾";
    }
}