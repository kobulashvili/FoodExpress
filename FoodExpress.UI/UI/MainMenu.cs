using FoodExpress.UI.Helpers;

namespace FoodExpress.UI.UI;

public class MainMenu
{
    private readonly AuthMenu _authMenu;
    private readonly AdminMenu _adminMenu;
    private readonly CustomerMenu _customerMenu;

    public MainMenu(
        AuthMenu authMenu,
        AdminMenu adminMenu,
        CustomerMenu customerMenu)
    {
        _authMenu = authMenu;
        _adminMenu = adminMenu;
        _customerMenu = customerMenu;
    }

    public async Task ShowAsync()
    {
        while (true)
        {
            ConsoleHelper.ShowTitle("FoodExpress");

            Console.WriteLine("1. Admin");
            Console.WriteLine("2. Customer");
            Console.WriteLine("0. Exit");
            Console.WriteLine();

            var choice = InputHelper.ReadString("Choose an option: ");

            try
            {
                switch (choice)
                {
                    case "1":
                        if (_authMenu.LoginAdmin())
                        {
                            await _adminMenu.ShowAsync();
                        }
                        break;

                    case "2":
                        var customer = await _authMenu.ShowCustomerAuthAsync();

                        if (customer != null)
                        {
                            await _customerMenu.ShowAsync(customer);
                        }
                        break;

                    case "0":
                        return;

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
}