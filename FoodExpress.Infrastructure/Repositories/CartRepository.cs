using FoodExpress.Domain.Entity;
using FoodExpress.Domain.Interfaces.Repositories;
using FoodExpress.Infrastructure.Services;

namespace FoodExpress.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly FileManager _fileManager;

    private const string FileName = "CartItems.txt";

    public CartRepository(FileManager fileManager)
    {
        _fileManager = fileManager;
    }

    public async Task<List<(string ItemId, int Quantity)>> GetItemsAsync(
        int userId)
    {
        var lines =
            await _fileManager.ReadAllLinesAsync(FileName);

        var result =
            new List<(string ItemId, int Quantity)>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts =
                line.Split('|');

            if (parts.Length != 3)
                continue;

            if (!int.TryParse(
                    parts[0].Trim(),
                    out var savedUserId))
                continue;

            if (savedUserId != userId)
                continue;

            var itemId =
                parts[1].Trim();

            if (!int.TryParse(
                    parts[2].Trim(),
                    out var quantity))
                continue;

            result.Add((itemId, quantity));
        }

        return result;
    }

    public async Task SaveAsync(
        int userId,
        Cart cart)
    {
        var lines =
            await _fileManager.ReadAllLinesAsync(FileName);

        lines = lines
            .Where(line =>
            {
                if (string.IsNullOrWhiteSpace(line))
                    return false;

                var parts =
                    line.Split('|');

                if (parts.Length != 3)
                    return true;

                return !int.TryParse(
                           parts[0].Trim(),
                           out var savedUserId)
                       || savedUserId != userId;
            })
            .ToList();

        foreach (var item in cart.Items)
        {
            lines.Add(
                $"{userId} | {item.Item.Id} | {item.Quantity}");
        }

        await _fileManager.WriteAllLinesAsync(
            FileName,
            lines);
    }

    public async Task DeleteAsync(int userId)
    {
        var lines =
            await _fileManager.ReadAllLinesAsync(FileName);

        lines = lines
            .Where(line =>
            {
                if (string.IsNullOrWhiteSpace(line))
                    return false;

                var parts =
                    line.Split('|');

                if (parts.Length != 3)
                    return true;

                return !int.TryParse(
                           parts[0].Trim(),
                           out var savedUserId)
                       || savedUserId != userId;
            })
            .ToList();

        await _fileManager.WriteAllLinesAsync(
            FileName,
            lines);
    }
}