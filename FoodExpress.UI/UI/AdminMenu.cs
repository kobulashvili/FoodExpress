
using FoodExpress.UI.Helpers;

namespace FoodExpress.UI.UI;

public class AdminMenu
{
    private readonly MenuUI _menuUI;
    private readonly RestaurantUI _restaurantUI;
    private readonly OrderUI _orderUI;
    private readonly PromoCodeUI _promoCodeUI;

    public AdminMenu(
        MenuUI menuUI,
        RestaurantUI restaurantUI,
        OrderUI orderUI,
        PromoCodeUI promoCodeUI)
    {
        _menuUI = menuUI;
        _restaurantUI = restaurantUI;
        _orderUI = orderUI;
        _promoCodeUI = promoCodeUI;
    }

    public async Task ShowAsync()
    {
        while (true)
        {
            ConsoleHelper.ShowTitle("Admin Menu");

            Console.WriteLine("1. Menu Management");
            Console.WriteLine("2. Restaurant Management");
            Console.WriteLine("3. Order Management");
            Console.WriteLine("4. Promo Code Management");
            Console.WriteLine("0. Logout");
            Console.WriteLine();

            var choice =
                InputHelper.ReadString("Choose an option: ");

            try
            {
                switch (choice)
                {
                    case "1":
                        await _menuUI.ShowAdminAsync();
                        break;

                    case "2":
                        await _restaurantUI.ShowAdminAsync();
                        break;

                    case "3":
                        await _orderUI.ShowAdminAsync();
                        break;

                    case "4":
                        await _promoCodeUI.ShowAsync();
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
