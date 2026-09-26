using FoodExpress.Domain.Enums;

namespace FoodExpress.Domain.Entity;

public abstract class User
{
    public int Id { get; private set; }
    public string Username { get; private set; }
    public string Password { get; private set; }
    public string Email { get; private set; }
    public UserRole Role { get; protected set; }

    protected User(
        int id,
        string username,
        string password,
        string email,
        UserRole role)
    {
        Id = id;
        Username = username;
        Password = password;
        Email = email;
        Role = role;
    }
}