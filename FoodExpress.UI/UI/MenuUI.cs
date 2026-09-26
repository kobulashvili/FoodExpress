using FoodExpress.Domain.Entity;
using FoodExpress.Service.Interfaces;
using FoodExpress.Service.Services;
using FoodExpress.UI.Helpers;

namespace FoodExpress.UI.UI;

public class MenuUI
{
    private readonly IMenuService _menuService;
    private readonly IAdminService _adminService;
    private readonly ICartService _cartService;

    public MenuUI(
    IMenuService menuService,
    IAdminService adminService,
    ICartService cartService)
    {
        _menuService = menuService;
        _adminService = adminService;
        _cartService = cartService;
    }


    public async Task ShowCustomerAsync(int userId)
    {
        while (true)
        {
            ConsoleHelper.ShowTitle("Menu");

            var items =
                await _menuService.GetAllAsync();

            DisplayItems(items);

            Console.WriteLine();
            Console.WriteLine("1. Add item to cart");
            Console.WriteLine("0. Back");
            Console.WriteLine();

            var choice =
                InputHelper.ReadString("Choose an option: ");

            try
            {
                switch (choice)
                {
                    case "1":
                        await AddItemToCartAsync(userId);
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


    public async Task ShowFilterAsync(int userId)
    {
        ConsoleHelper.ShowTitle("Filter Menu");

        Console.WriteLine(
            "Leave a value empty if you do not want to filter by it.");
        Console.WriteLine();

        Console.Write("Minimum price: ");
        var minInput = Console.ReadLine();

        Console.Write("Maximum price: ");
        var maxInput = Console.ReadLine();

        Console.Write("Category: ");
        var category = Console.ReadLine();

        Console.Write("Restaurant ID: ");
        var restaurantInput = Console.ReadLine();

        decimal? minPrice = null;
        decimal? maxPrice = null;
        int? restaurantId = null;

        if (!string.IsNullOrWhiteSpace(minInput))
        {
            if (!decimal.TryParse(
                    minInput,
                    out var value))
            {
                ConsoleHelper.ShowError(
                    "Invalid minimum price.");

                ConsoleHelper.Pause();
                return;
            }

            minPrice = value;
        }

        if (!string.IsNullOrWhiteSpace(maxInput))
        {
            if (!decimal.TryParse(
                    maxInput,
                    out var value))
            {
                ConsoleHelper.ShowError(
                    "Invalid maximum price.");

                ConsoleHelper.Pause();
                return;
            }

            maxPrice = value;
        }

        if (!string.IsNullOrWhiteSpace(restaurantInput))
        {
            if (!int.TryParse(
                    restaurantInput,
                    out var value))
            {
                ConsoleHelper.ShowError(
                    "Invalid restaurant ID.");

                ConsoleHelper.Pause();
                return;
            }

            restaurantId = value;
        }

        var items =
            await _menuService.FilterAsync(
                minPrice,
                maxPrice,
                category,
                restaurantId);

        Console.WriteLine();

        DisplayItems(items);

        ConsoleHelper.Pause();
    }

    public async Task ShowAdminAsync()
    {
        while (true)
        {
            ConsoleHelper.ShowTitle(
                "Menu Management");

            Console.WriteLine("1. View Menu");
            Console.WriteLine("2. Add Menu Item");
            Console.WriteLine("3. Update Menu Item");
            Console.WriteLine("4. Delete Menu Item");
            Console.WriteLine("0. Back");
            Console.WriteLine();

            var choice =
                InputHelper.ReadString("Choose an option: ");

            try
            {
                switch (choice)
                {
                    case "1":
                        await DisplayAdminMenuAsync();
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

    private async Task DisplayAdminMenuAsync()
    {
        var items =
            await _adminService.GetMenuAsync();

        DisplayItems(items);

        ConsoleHelper.Pause();
    }

    private async Task AddAsync()
    {
        ConsoleHelper.ShowTitle(
            "Add Menu Item");

        Console.WriteLine("1. Pizza");
        Console.WriteLine("2. Drink");
        Console.WriteLine("3. Dessert");
        Console.WriteLine();

        var type =
            InputHelper.ReadString("Choose type: ");

        var id =
            InputHelper.ReadString("Item ID: ");

        var restaurantId =
            InputHelper.ReadPositiveInt(
                "Restaurant ID: ");

        var name =
            InputHelper.ReadString("Name: ");

        var category =
            InputHelper.ReadString("Category: ");

        var price =
            InputHelper.ReadPositiveDecimal(
                "Price: ");

        MenuItem item;

        switch (type)
        {
            case "1":
                var size =
                    InputHelper.ReadString("Size: ");

                var dough =
                    InputHelper.ReadString("Dough: ");

                item = new Pizza(
                    id,
                    restaurantId,
                    name,
                    category,
                    price,
                    size,
                    dough);

                break;

            case "2":
                var volume =
                    InputHelper.ReadString("Volume: ");

                item = new Drink(
                    id,
                    restaurantId,
                    name,
                    category,
                    price,
                    volume);

                break;

            case "3":
                var portionSize =
                    InputHelper.ReadString(
                        "Portion size: ");

                item = new Dessert(
                    id,
                    restaurantId,
                    name,
                    category,
                    price,
                    portionSize);

                break;

            default:
                ConsoleHelper.ShowError(
                    "Invalid menu item type.");

                ConsoleHelper.Pause();
                return;
        }

        await _adminService.AddMenuItemAsync(item);

        ConsoleHelper.ShowSuccess(
            "Menu item added successfully.");

        ConsoleHelper.Pause();
    }

    private async Task UpdateAsync()
    {
        ConsoleHelper.ShowTitle(
            "Update Menu Item");

        var id =
            InputHelper.ReadString("Item ID: ");

        var existing =
            await _menuService.GetByIdAsync(id);

        if (existing == null)
            throw new KeyNotFoundException(
                "Menu item was not found.");

        Console.WriteLine(
            $"Current item: {existing.DisplayDetails()}");
        Console.WriteLine();

        var name =
            InputHelper.ReadString("Name: ");

        var category =
            InputHelper.ReadString("Category: ");

        var price =
            InputHelper.ReadPositiveDecimal(
                "Price: ");

        MenuItem updated;

        if (existing is Pizza pizza)
        {
            var size =
                InputHelper.ReadString(
                    $"Size ({pizza.Size}): ");

            var dough =
                InputHelper.ReadString(
                    $"Dough ({pizza.Dough}): ");

            updated = new Pizza(
                pizza.Id,
                pizza.RestaurantId,
                name,
                category,
                price,
                size,
                dough);
        }
        else if (existing is Drink drink)
        {
            var volume =
                InputHelper.ReadString(
                    $"Volume ({drink.Volume}): ");

            updated = new Drink(
                drink.Id,
                drink.RestaurantId,
                name,
                category,
                price,
                volume);
        }
        else if (existing is Dessert dessert)
        {
            var portion =
                InputHelper.ReadString(
                    $"Portion size ({dessert.PortionSize}): ");

            updated = new Dessert(
                dessert.Id,
                dessert.RestaurantId,
                name,
                category,
                price,
                portion);
        }
        else
        {
            throw new InvalidOperationException(
                "Unknown menu item type.");
        }

        await _adminService.UpdateMenuItemAsync(
            updated);

        ConsoleHelper.ShowSuccess(
            "Menu item updated successfully.");

        ConsoleHelper.Pause();
    }

    private async Task DeleteAsync()
    {
        ConsoleHelper.ShowTitle(
            "Delete Menu Item");

        var id =
            InputHelper.ReadString("Item ID: ");

        await _adminService.DeleteMenuItemAsync(id);

        ConsoleHelper.ShowSuccess(
            "Menu item deleted successfully.");

        ConsoleHelper.Pause();
    }

    private static void DisplayItems(
        IEnumerable<MenuItem> items)
    {
        var list = items.ToList();

        if (list.Count == 0)
        {
            Console.WriteLine("No menu items found.");
            return;
        }

        foreach (var item in list)
        {
            Console.WriteLine(
                $"ID: {item.Id} | " +
                $"Restaurant: {item.RestaurantId} | " +
                $"Category: {item.Category} | " +
                $"Available: {item.IsAvailable}");

            Console.WriteLine(
                item.DisplayDetails());

            Console.WriteLine(
                "----------------------------------------");
        }
    }



    private async Task AddItemToCartAsync(int userId)
    {
        var itemId =
            InputHelper.ReadString("Item ID: ");

        var item =
            await _menuService.GetByIdAsync(itemId);

        if (item == null)
            throw new KeyNotFoundException(
                "Menu item was not found.");

        if (!item.IsAvailable)
            throw new InvalidOperationException(
                "This item is not available.");

        var quantity =
            InputHelper.ReadPositiveInt(
                "Quantity: ");

        await _cartService.AddItemAsync(
            userId,
            item,
            quantity);

        ConsoleHelper.ShowSuccess(
            "Item added to cart.");

        ConsoleHelper.Pause();
    }
}