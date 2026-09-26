using FoodExpress.Domain.Entity;
using FoodExpress.Domain.Enums;
using FoodExpress.Domain.Interfaces.Repositories;
using FoodExpress.Infrastructure.Services;

namespace FoodExpress.Infrastructure.Repositories;

public class MenuRepository : IMenuRepository
{
    private readonly FileManager _fileManager;

    private const string FileName = "MenuItems.txt";

    public MenuRepository(FileManager fileManager)
    {
        _fileManager = fileManager;
    }

    public async Task<List<MenuItem>> GetAllAsync()
    {
        try
        {
            var lines = await _fileManager.ReadAllLinesAsync(FileName);
            var items = new List<MenuItem>();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                try
                {
                    var item = ParseMenuItem(line);

                    if (item != null)
                        items.Add(item);
                }
                catch
                {
                    // Ignore invalid lines and continue.
                }
            }

            return items;
        }
        catch (Exception ex)
        {
            throw new Exception(
                "Failed to load menu items from MenuItems.txt.",
                ex);
        }
    }

    public async Task<MenuItem?> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        var items = await GetAllAsync();

        return items.FirstOrDefault(
            x => x.Id.Equals(
                id,
                StringComparison.OrdinalIgnoreCase));
    }

    public async Task AddAsync(MenuItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        try
        {
            var items = await GetAllAsync();

            if (items.Any(x =>
                    x.Id.Equals(
                        item.Id,
                        StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    "A menu item with this ID already exists.");
            }

            items.Add(item);

            await SaveAllAsync(items);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception(
                "Failed to add menu item.",
                ex);
        }
    }

    public async Task UpdateAsync(MenuItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        try
        {
            var items = await GetAllAsync();

            var index = items.FindIndex(
                x => x.Id.Equals(
                    item.Id,
                    StringComparison.OrdinalIgnoreCase));

            if (index == -1)
                throw new KeyNotFoundException(
                    "Menu item was not found.");

            items[index] = item;

            await SaveAllAsync(items);
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception(
                "Failed to update menu item.",
                ex);
        }
    }

    public async Task DeleteAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException(
                "Menu item ID is required.",
                nameof(id));

        try
        {
            var items = await GetAllAsync();

            var item = items.FirstOrDefault(
                x => x.Id.Equals(
                    id,
                    StringComparison.OrdinalIgnoreCase));

            if (item == null)
                throw new KeyNotFoundException(
                    "Menu item was not found.");

            items.Remove(item);

            await SaveAllAsync(items);
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception(
                "Failed to delete menu item.",
                ex);
        }
    }

    private static MenuItem? ParseMenuItem(string line)
    {
        var parts = line.Split('|');

        if (parts.Length < 8)
            return null;

        var id = parts[0].Trim();
        var restaurantIdText = parts[1].Trim();
        var typeText = parts[2].Trim();
        var name = parts[3].Trim();
        var category = parts[4].Trim();
        var details = parts[5].Trim();
        var priceText = parts[6].Trim();
        var availableText = parts[7].Trim();

        if (string.IsNullOrWhiteSpace(id) ||
            string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(category))
        {
            return null;
        }

        if (!int.TryParse(
                restaurantIdText,
                out var restaurantId))
        {
            return null;
        }

        if (!decimal.TryParse(
                priceText,
                out var price))
        {
            return null;
        }

        if (!bool.TryParse(
                availableText,
                out var isAvailable))
        {
            return null;
        }

        if (!Enum.TryParse<MenuItemType>(
                typeText,
                true,
                out var type))
        {
            return null;
        }

        return type switch
        {
            MenuItemType.Pizza =>
                ParsePizza(
                    id,
                    restaurantId,
                    name,
                    category,
                    price,
                    isAvailable,
                    details),

            MenuItemType.Drink =>
                ParseDrink(
                    id,
                    restaurantId,
                    name,
                    category,
                    price,
                    isAvailable,
                    details),

            MenuItemType.Dessert =>
                ParseDessert(
                    id,
                    restaurantId,
                    name,
                    category,
                    price,
                    isAvailable,
                    details),

            _ => null
        };
    }

    private static Pizza? ParsePizza(
        string id,
        int restaurantId,
        string name,
        string category,
        decimal price,
        bool isAvailable,
        string details)
    {
        var parts = details.Split(';');

        var size = parts.Length > 0
            ? parts[0].Trim()
            : string.Empty;

        var dough = parts.Length > 1
            ? parts[1].Trim()
            : string.Empty;

        if (string.IsNullOrWhiteSpace(size) ||
            string.IsNullOrWhiteSpace(dough))
        {
            return null;
        }

        var pizza = new Pizza(
            id,
            restaurantId,
            name,
            category,
            price,
            size,
            dough);

        if (!isAvailable)
            pizza.Disable();

        return pizza;
    }

    private static Drink? ParseDrink(
    string id,
    int restaurantId,
    string name,
    string category,
    decimal price,
    bool isAvailable,
    string details)
    {
        if (string.IsNullOrWhiteSpace(details))
            return null;

        var drink = new Drink(
            id,
            restaurantId,
            name,
            category,
            price,
            details);

        if (!isAvailable)
            drink.Disable();

        return drink;
    }

    private static Dessert? ParseDessert(
        string id,
        int restaurantId,
        string name,
        string category,
        decimal price,
        bool isAvailable,
        string details)
    {
        var dessert = new Dessert(
            id,
            restaurantId,
            name,
            category,
            price,
            details);

        if (!isAvailable)
            dessert.Disable();

        return dessert;
    }

    private async Task SaveAllAsync(List<MenuItem> items)
    {
        var lines = items.Select(FormatMenuItem);

        await _fileManager.WriteAllLinesAsync(
            FileName,
            lines);
    }

    private static string FormatMenuItem(MenuItem item)
    {
        var details = item switch
        {
            Pizza pizza =>
                $"{pizza.Size};{pizza.Dough}",

            Drink drink =>
                drink.Volume.ToString(),

            Dessert dessert =>
                dessert.PortionSize,

            _ => string.Empty
        };

        var type = item switch
        {
            Pizza => MenuItemType.Pizza,
            Drink => MenuItemType.Drink,
            Dessert => MenuItemType.Dessert,
            _ => throw new InvalidOperationException(
                "Unknown menu item type.")
        };

        return string.Join(
            " | ",
            item.Id,
            item.RestaurantId,
            type,
            item.Name,
            item.Category,
            details,
            item.Price,
            item.IsAvailable);
    }
}