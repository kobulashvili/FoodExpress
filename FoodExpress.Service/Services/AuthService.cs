using FoodExpress.Domain.Entity;
using FoodExpress.Service.Interfaces;

namespace FoodExpress.Service.Services;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly FoodExpress.Domain.Interfaces.IEmailService _emailService;

    private readonly Dictionary<string, PendingRegistration>
        _pendingRegistrations = new();

    public AuthService(
        IUserService userService,
        FoodExpress.Domain.Interfaces.IEmailService emailService)
    {
        _userService = userService;
        _emailService = emailService;
    }

    public async Task<User?> LoginAsync(
        string username,
        string password)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException(
                "Username is required.");

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException(
                "Password is required.");

        var user =
            await _userService.GetByUsernameAsync(username);

        if (user == null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(
         password,
         user.Password))
        {
            return null;
        }

        return user;
    }

    public async Task<Customer> RegisterAsync(
        string username,
        string password,
        string email,
        string address)
    {
        return await VerifyRegistrationAsync(
            email,
            "");
    }

    public async Task<string> StartRegistrationAsync(
        string username,
        string password,
        string email,
        string address)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException(
                "Username is required.");

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException(
                "Password is required.");

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException(
                "Email is required.");

        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException(
                "Address is required.");

        var existingUsername =
            await _userService.GetByUsernameAsync(username);

        if (existingUsername != null)
            throw new InvalidOperationException(
                "Username already exists.");

        var existingEmail =
            await _userService.GetByEmailAsync(email);

        if (existingEmail != null)
            throw new InvalidOperationException(
                "Email already exists.");

        var code =
            Random.Shared.Next(100000, 1000000)
                .ToString();

        _pendingRegistrations[email] =
            new PendingRegistration(
                username,
                password,
                email,
                address,
                code);

        await _emailService.SendVerificationCodeAsync(
            email,
            code);

        return code;
    }

    public async Task<Customer> VerifyRegistrationAsync(
        string email,
        string code)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException(
                "Email is required.");

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Verification code is required.");

        if (!_pendingRegistrations.TryGetValue(
                email,
                out var pending))
        {
            throw new InvalidOperationException(
                "Registration request was not found.");
        }

        if (pending.Code != code)
            throw new InvalidOperationException(
                "Invalid verification code.");

        var existingUsername =
            await _userService.GetByUsernameAsync(
                pending.Username);

        if (existingUsername != null)
            throw new InvalidOperationException(
                "Username already exists.");

        var existingEmail =
            await _userService.GetByEmailAsync(
                pending.Email);

        if (existingEmail != null)
            throw new InvalidOperationException(
                "Email already exists.");

        var users =
            await _userService.GetAllAsync();

        var userId = users.Count == 0
            ? 1001
            : users.Max(x => x.Id) + 1;

        // Hash the password before creating the user.
        var hashedPassword =
     BCrypt.Net.BCrypt.HashPassword(pending.Password);

        var customer = new Customer(
            userId,
            pending.Username,
            hashedPassword,
            pending.Email,
            pending.Address);

        await _userService.AddAsync(customer);

        _pendingRegistrations.Remove(email);

        return customer;
    }

    private class PendingRegistration
    {
        public string Username { get; }
        public string Password { get; }
        public string Email { get; }
        public string Address { get; }
        public string Code { get; }

        public PendingRegistration(
            string username,
            string password,
            string email,
            string address,
            string code)
        {
            Username = username;
            Password = password;
            Email = email;
            Address = address;
            Code = code;
        }
    }
}
