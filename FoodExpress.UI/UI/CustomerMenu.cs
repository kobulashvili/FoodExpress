using FoodExpress.Domain.Entity;
using FoodExpress.Service.Interfaces;
using FoodExpress.UI.Helpers;

namespace FoodExpress.UI.UI;

public class CustomerMenu
{
    private readonly MenuUI _menuUI;
    private readonly CartUI _cartUI;
    private readonly OrderUI _orderUI;
    private readonly IRestaurantService _restaurantService;

    public CustomerMenu(
        MenuUI menuUI,
        CartUI cartUI,
        OrderUI orderUI,
        IRestaurantService restaurantService)
    {
        _menuUI = menuUI;
        _cartUI = cartUI;
        _orderUI = orderUI;
        _restaurantService = restaurantService;
    }

    public async Task ShowAsync(Customer customer)
    {
        while (true)
        {
            ConsoleHelper.ShowTitle(
                $"Customer Menu - {customer.Username}");

            Console.WriteLine("1. View Restaurants");
            Console.WriteLine("2. Browse Menu");
            Console.WriteLine("3. Filter Menu");
            Console.WriteLine("4. Cart");
            Console.WriteLine("5. My Orders");
            Console.WriteLine("0. Logout");
            Console.WriteLine();

            var choice =
                InputHelper.ReadString("Choose an option: ");

            try
            {
                switch (choice)
                {
                    case "1":
                        await DisplayRestaurantsAsync();
                        break;

                    case "2":
                        await _menuUI.ShowCustomerAsync(
                            customer.Id);
                        break;

                    case "3":
                        await _menuUI.ShowFilterAsync(
                            customer.Id);
                        break;

                    case "4":
                        await _cartUI.ShowAsync(customer);
                        break;

                    case "5":
                        await _orderUI.ShowCustomerAsync(
                            customer);
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

    private async Task DisplayRestaurantsAsync()
    {
        ConsoleHelper.ShowTitle(
            "Restaurants");

        var restaurants =
            await _restaurantService.GetAllAsync();

        if (restaurants.Count == 0)
        {
            Console.WriteLine(
                "No restaurants found.");

            ConsoleHelper.Pause();
            return;
        }

        foreach (var restaurant in restaurants)
        {
            Console.WriteLine(
                $"ID: {restaurant.Id}");

            Console.WriteLine(
                $"Name: {restaurant.Name}");

            Console.WriteLine(
                $"Address: {restaurant.Address}");

            Console.WriteLine(
                $"Status: " +
                $"{(restaurant.IsActive ? "Active" : "Inactive")}");

            Console.WriteLine(
                "----------------------------------------");
        }

        ConsoleHelper.Pause();
    }
}
