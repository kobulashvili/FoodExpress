namespace FoodExpress.Domain.Entity;

public class Restaurant
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Address { get; private set; }
    public bool IsActive { get; private set; }

    public Restaurant(
        int id,
        string name,
        string address)
    {
        Id = id;
        Name = name;
        Address = address;
        IsActive = true;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}