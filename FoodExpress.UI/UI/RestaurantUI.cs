using FoodExpress.Domain.Entity;
using FoodExpress.Service.Interfaces;
using FoodExpress.UI.Helpers;

namespace FoodExpress.UI.UI;

public class RestaurantUI
{
    private readonly IRestaurantService _restaurantService;

    public RestaurantUI(
        IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    public async Task ShowAdminAsync()
    {
        while (true)
        {
            ConsoleHelper.ShowTitle(
                "Restaurant Management");

            Console.WriteLine("1. View Restaurants");
            Console.WriteLine("2. Add Restaurant");
            Console.WriteLine("3. Update Restaurant");
            Console.WriteLine("4. Delete Restaurant");
            Console.WriteLine("5. Activate Restaurant");
            Console.WriteLine("6. Deactivate Restaurant");
            Console.WriteLine("0. Back");
            Console.WriteLine();

            var choice =
                InputHelper.ReadString("Choose an option: ");

            try
            {
                switch (choice)
                {
                    case "1":
                        await DisplayAsync();
                        break;

                    case "2":
                        await AddAsync();
                        break;

                    case "3":
                        await UpdateAsync();
                        break;

                    case "4":
                        await DeleteAsync();
                        break;

                    case "5":
                        await ActivateAsync();
                        break;

                    case "6":
                        await DeactivateAsync();
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

    private async Task DisplayAsync()
    {
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

    private async Task AddAsync()
    {
        ConsoleHelper.ShowTitle(
            "Add Restaurant");

        var id =
            InputHelper.ReadPositiveInt("ID: ");

        var name =
            InputHelper.ReadString("Name: ");

        var address =
            InputHelper.ReadString("Address: ");

        var restaurant =
            new Restaurant(
                id,
                name,
                address);

        await _restaurantService.AddAsync(
            restaurant);

        ConsoleHelper.ShowSuccess(
            "Restaurant added successfully.");

        ConsoleHelper.Pause();
    }

    private async Task UpdateAsync()
    {
        ConsoleHelper.ShowTitle(
            "Update Restaurant");

        var id =
            InputHelper.ReadPositiveInt("ID: ");

        var existing =
            await _restaurantService.GetByIdAsync(id);

        if (existing == null)
            throw new KeyNotFoundException(
                "Restaurant was not found.");

        var name =
            InputHelper.ReadString("Name: ");

        var address =
            InputHelper.ReadString("Address: ");

        var updated =
            new Restaurant(
                existing.Id,
                name,
                address);

        if (!existing.IsActive)
            updated.Deactivate();

        await _restaurantService.UpdateAsync(
            updated);

        ConsoleHelper.ShowSuccess(
            "Restaurant updated successfully.");

        ConsoleHelper.Pause();
    }

    private async Task DeleteAsync()
    {
        ConsoleHelper.ShowTitle(
            "Delete Restaurant");

        var id =
            InputHelper.ReadPositiveInt("ID: ");

        await _restaurantService.DeleteAsync(id);

        ConsoleHelper.ShowSuccess(
            "Restaurant deleted successfully.");

        ConsoleHelper.Pause();
    }

    private async Task ActivateAsync()
    {
        var id =
            InputHelper.ReadPositiveInt(
                "Restaurant ID: ");

        await _restaurantService.ActivateAsync(id);

        ConsoleHelper.ShowSuccess(
            "Restaurant activated successfully.");

        ConsoleHelper.Pause();
    }

    private async Task DeactivateAsync()
    {
        var id =
            InputHelper.ReadPositiveInt(
                "Restaurant ID: ");

        await _restaurantService.DeactivateAsync(id);

        ConsoleHelper.ShowSuccess(
            "Restaurant deactivated successfully.");

        ConsoleHelper.Pause();
    }
}