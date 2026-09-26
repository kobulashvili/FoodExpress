
using FoodExpress.Domain.Entity;
using FoodExpress.Domain.Enums;
using FoodExpress.Domain.Interfaces.Repositories;
using FoodExpress.Infrastructure.Services;

namespace FoodExpress.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly FileManager _fileManager;

    private const string FileName = "Users.txt";

    public UserRepository(FileManager fileManager)
    {
        _fileManager = fileManager;
    }

    public async Task<List<User>> GetAllAsync()
    {
        try
        {
            var lines = await _fileManager.ReadAllLinesAsync(FileName);
            var users = new List<User>();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                try
                {
                    var user = ParseUser(line);

                    if (user != null)
                        users.Add(user);
                }
                catch
                {
                    // Ignore invalid lines and continue reading
                    // the remaining users.
                }
            }

            return users;
        }
        catch (Exception ex)
        {
            throw new Exception(
                "Failed to load users from Users.txt.",
                ex);
        }
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        var users = await GetAllAsync();

        return users.FirstOrDefault(x => x.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return null;

        var users = await GetAllAsync();

        return users.FirstOrDefault(
            x => x.Username.Equals(
                username,
                StringComparison.OrdinalIgnoreCase));
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        var users = await GetAllAsync();

        return users.FirstOrDefault(
            x => x.Email.Equals(
                email,
                StringComparison.OrdinalIgnoreCase));
    }

    public async Task AddAsync(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        try
        {
            var users = await GetAllAsync();

            if (users.Any(x => x.Id == user.Id))
                throw new InvalidOperationException(
                    "A user with this ID already exists.");

            if (users.Any(x =>
                    x.Username.Equals(
                        user.Username,
                        StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    "A user with this username already exists.");
            }

            users.Add(user);

            await SaveAllAsync(users);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception(
                "Failed to add user.",
                ex);
        }
    }

    public async Task UpdateAsync(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        try
        {
            var users = await GetAllAsync();

            var index = users.FindIndex(
                x => x.Id == user.Id);

            if (index == -1)
                throw new KeyNotFoundException(
                    "User was not found.");

            users[index] = user;

            await SaveAllAsync(users);
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception(
                "Failed to update user.",
                ex);
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var users = await GetAllAsync();

            var user = users.FirstOrDefault(
                x => x.Id == id);

            if (user == null)
                throw new KeyNotFoundException(
                    "User was not found.");

            users.Remove(user);

            await SaveAllAsync(users);
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception(
                "Failed to delete user.",
                ex);
        }
    }

    private static User? ParseUser(string line)
    {
        var parts = line.Split('|');

        if (parts.Length != 5)
            return null;

        var idText = parts[0].Trim();
        var username = parts[1].Trim();
        var password = parts[2].Trim();
        var email = parts[3].Trim();
        var roleText = parts[4].Trim();

        if (!int.TryParse(idText, out var id))
            return null;

        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        if (!Enum.TryParse<UserRole>(
                roleText,
                true,
                out var role))
        {
            return null;
        }

        return role switch
        {
            UserRole.Customer =>
                new Customer(
                    id,
                    username,
                    password,
                    email,
                    string.Empty),

            UserRole.Admin =>
                new Admin(
                    id,
                    username,
                    password,
                    email),

            _ => null
        };
    }

    private async Task SaveAllAsync(List<User> users)
    {
        var lines = users.Select(user =>
            FormatUser(user));

        await _fileManager.WriteAllLinesAsync(
            FileName,
            lines);
    }

    private static string FormatUser(User user)
    {
        return string.Join(
            " | ",
            user.Id,
            user.Username,
            user.Password,
            user.Email,
            user.Role);
    }
}

