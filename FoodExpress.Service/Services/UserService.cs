
using BCrypt.Net;
using FoodExpress.Service.Interfaces;
using FoodExpress.Domain.Interfaces.Repositories;
using FoodExpress.Domain.Entity;

namespace FoodExpress.Service.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required.");

        return await _userRepository.GetByUsernameAsync(username);
    }

    public async Task AddAsync(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var existingUser =
            await _userRepository.GetByUsernameAsync(user.Username);

        if (existingUser != null)
            throw new InvalidOperationException(
                "Username already exists.");

        await _userRepository.AddAsync(user);
    }

    public async Task UpdateAsync(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var existingUser =
            await _userRepository.GetByIdAsync(user.Id);

        if (existingUser == null)
            throw new KeyNotFoundException(
                "User was not found.");

        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            throw new KeyNotFoundException(
                "User was not found.");

        await _userRepository.DeleteAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException(
                "Email is required.");

        return await _userRepository.GetByEmailAsync(email);
    }
}
