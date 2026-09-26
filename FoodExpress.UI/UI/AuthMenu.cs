
using FoodExpress.Domain.Entity;
using FoodExpress.Service.Interfaces;
using FoodExpress.UI.Helpers;

namespace FoodExpress.UI.UI;

public class AuthMenu
{
    private readonly IAuthService _authService;

    public AuthMenu(IAuthService authService)
    {
        _authService = authService;
    }

    public bool LoginAdmin()
    {
        ConsoleHelper.ShowTitle("Admin Login");

        var username =
            InputHelper.ReadString("Username: ");

        var password =
            InputHelper.ReadPassword("Password: ");

        if (username == "admin" &&
            password == "admin")
        {
            ConsoleHelper.ShowSuccess(
                "Admin login successful.");

            ConsoleHelper.Pause();

            return true;
        }

        ConsoleHelper.ShowError(
            "Invalid username or password.");

        ConsoleHelper.Pause();

        return false;
    }

    public async Task<Customer?> ShowCustomerAuthAsync()
    {
        while (true)
        {
            ConsoleHelper.ShowTitle("Customer");

            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register");
            Console.WriteLine("0. Back");
            Console.WriteLine();

            var choice =
                InputHelper.ReadString("Choose an option: ");

            try
            {
                switch (choice)
                {
                    case "1":
                        var customer = await LoginAsync();

                        if (customer != null)
                            return customer;

                        break;

                    case "2":
                        var registeredCustomer =
                            await RegisterAsync();

                        if (registeredCustomer != null)
                            return registeredCustomer;

                        break;

                    case "0":
                        return null;

                    default:
                        ConsoleHelper.ShowError(
                            "Invalid option.");

                        ConsoleHelper.Pause();
                        break;
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.ShowError(ex.Message);
                ConsoleHelper.Pause();
            }
        }
    }

    private async Task<Customer?> LoginAsync()
    {
        ConsoleHelper.ShowTitle("Customer Login");

        var username =
            InputHelper.ReadString("Username: ");

        var password =
            InputHelper.ReadPassword("Password: ");

        var user =
            await _authService.LoginAsync(
                username,
                password);

        if (user == null)
        {
            ConsoleHelper.ShowError(
                "Invalid username or password.");

            ConsoleHelper.Pause();

            return null;
        }

        if (user is not Customer customer)
        {
            ConsoleHelper.ShowError(
                "This account is not a customer account.");

            ConsoleHelper.Pause();

            return null;
        }

        ConsoleHelper.ShowSuccess(
            "Login successful.");

        ConsoleHelper.Pause();

        return customer;
    }

  
private async Task<Customer?> RegisterAsync()
    {
        ConsoleHelper.ShowTitle("Customer Registration");

        var username =
            InputHelper.ReadString("Username: ");

        var password =
            InputHelper.ReadPassword("Password: ");

        var email =
            InputHelper.ReadString("Email: ");

        var address =
            InputHelper.ReadString("Address: ");

        await _authService.StartRegistrationAsync(
            username,
            password,
            email,
            address);

        ConsoleHelper.ShowSuccess(
            "Verification code has been sent to your email.");

        var code =
            InputHelper.ReadString(
                "Verification code: ");

        var customer =
            await _authService.VerifyRegistrationAsync(
                email,
                code);

        ConsoleHelper.ShowSuccess(
            "Registration successful.");

        ConsoleHelper.Pause();

        return customer;
    }

    
}
