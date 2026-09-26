using FoodExpress.Domain.Entity;
using FoodExpress.Domain.Interfaces;
using FoodExpress.Service.Interfaces;

namespace FoodExpress.Service.Services;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;

    private readonly Dictionary<string, PendingRegistration>
        _pendingRegistrations = new();

    public AuthService(
        IUserService userService,
        IEmailService emailService)
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

        if (user.Password != password)
            return null;

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

        var existingUser =
            await _userService.GetByUsernameAsync(username);

        if (existingUser != null)
            throw new InvalidOperationException(
                "Username already exists.");

        var existingEmail =
            await _userService.GetByEmailAsync(email);

        if (existingEmail != null)
            throw new InvalidOperationException(
                "Email already exists.");

        var code =
            Random.Shared.Next(
                100000,
                1000000)
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
                out var registration))
        {
            throw new InvalidOperationException(
                "No pending registration found.");
        }

        if (registration.Code != code)
            throw new InvalidOperationException(
                "Invalid verification code.");

        var existingUser =
            await _userService.GetByUsernameAsync(
                registration.Username);

        if (existingUser != null)
            throw new InvalidOperationException(
                "Username already exists.");

        var existingEmail =
            await _userService.GetByEmailAsync(
                registration.Email);

        if (existingEmail != null)
            throw new InvalidOperationException(
                "Email already exists.");

        var existingUsers =
            await _userService.GetAllAsync();

        var newId = existingUsers.Count == 0
            ? 1001
            : existingUsers.Max(x => x.Id) + 1;

        var customer = new Customer(
            newId,
            registration.Username,
            registration.Password,
            registration.Email,
            registration.Address);

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