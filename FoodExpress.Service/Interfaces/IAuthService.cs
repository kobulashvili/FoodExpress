
using FoodExpress.Domain.Entity;

namespace FoodExpress.Service.Interfaces;

public interface IAuthService
{
    Task<User?> LoginAsync(
        string username,
        string password);

    Task<Customer> RegisterAsync(
        string username,
        string password,
        string email,
        string address);

    Task<string> StartRegistrationAsync(
        string username,
        string password,
        string email,
        string address);

    Task<Customer> VerifyRegistrationAsync(
        string email,
        string code);
}

