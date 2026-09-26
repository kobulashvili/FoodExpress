using FoodExpress.Domain.Enums;

namespace FoodExpress.Domain.Entity;

public class Customer : User
{
    public string Address { get; private set; }

    public Customer(
        int id,
        string username,
        string password,
        string email,
        string address)
        : base(id, username, password, email, UserRole.Customer)
    {
        Address = address;
    }
}