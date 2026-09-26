using FoodExpress.Domain.Enums;

namespace FoodExpress.Domain.Entity;

public class Admin : User
{
    public Admin(
        int id,
        string username,
        string password,
        string email)
        : base(id, username, password, email, UserRole.Admin)
    {
    }
}