using FoodExpress.Domain.Entity;

namespace FoodExpress.Service.Interfaces;

public interface IUserService
{
    Task<List<User>> GetAllAsync();

    Task<User?> GetByIdAsync(int id);

    Task<User?> GetByUsernameAsync(string username);

    Task AddAsync(User user);

    Task UpdateAsync(User user);

    Task DeleteAsync(int id);

    Task<User?> GetByEmailAsync(string email);
}