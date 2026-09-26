using System.Globalization;
using FoodExpress.Domain.Entity;
using FoodExpress.Domain.Enums;
using FoodExpress.Domain.Interfaces.Repositories;
using FoodExpress.Infrastructure.Services;

namespace FoodExpress.Infrastructure.Repositories;

public class PromoCodeRepository : IPromoCodeRepository
{
    private readonly FileManager _fileManager;

    private const string FileName = "PromoCodes.txt";

    public PromoCodeRepository(FileManager fileManager)
    {
        _fileManager = fileManager;
    }

    public async Task<List<PromoCode>> GetAllAsync()
    {
        var lines = await _fileManager.ReadAllLinesAsync(FileName);

        var promoCodes = new List<PromoCode>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var promoCode = ParsePromoCode(line);

            if (promoCode != null)
                promoCodes.Add(promoCode);
        }

        return promoCodes;
    }

    public async Task<PromoCode?> GetByCodeAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        var promoCodes = await GetAllAsync();

        return promoCodes.FirstOrDefault(
            x => x.Code.Equals(
                code,
                StringComparison.OrdinalIgnoreCase));
    }

    public async Task AddAsync(PromoCode promoCode)
    {
        if (promoCode == null)
            throw new ArgumentNullException(nameof(promoCode));

        var promoCodes = await GetAllAsync();

        if (promoCodes.Any(x =>
                x.Code.Equals(
                    promoCode.Code,
                    StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                "Promo code already exists.");
        }

        promoCodes.Add(promoCode);

        await SaveAllAsync(promoCodes);
    }

    public async Task UpdateAsync(PromoCode promoCode)
    {
        if (promoCode == null)
            throw new ArgumentNullException(nameof(promoCode));

        var promoCodes = await GetAllAsync();

        var index = promoCodes.FindIndex(
            x => x.Code.Equals(
                promoCode.Code,
                StringComparison.OrdinalIgnoreCase));

        if (index == -1)
            throw new KeyNotFoundException(
                "Promo code was not found.");

        promoCodes[index] = promoCode;

        await SaveAllAsync(promoCodes);
    }

    public async Task DeleteAsync(string code)
    {
        var promoCodes = await GetAllAsync();

        var promoCode = promoCodes.FirstOrDefault(
            x => x.Code.Equals(
                code,
                StringComparison.OrdinalIgnoreCase));

        if (promoCode == null)
            throw new KeyNotFoundException(
                "Promo code was not found.");

        promoCodes.Remove(promoCode);

        await SaveAllAsync(promoCodes);
    }

    private static PromoCode? ParsePromoCode(string line)
    {
        var parts = line.Split('|');

        if (parts.Length != 6)
            return null;

        var code = parts[0].Trim();
        var typeText = parts[1].Trim();
        var valueText = parts[2].Trim();
        var validFromText = parts[3].Trim();
        var validUntilText = parts[4].Trim();
        var isActiveText = parts[5].Trim();

        if (!decimal.TryParse(
                valueText,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var value))
        {
            return null;
        }

        if (!DateTime.TryParse(
                validFromText,
                out var validFrom))
        {
            return null;
        }

        if (!DateTime.TryParse(
                validUntilText,
                out var validUntil))
        {
            return null;
        }

        if (!bool.TryParse(
                isActiveText,
                out var isActive))
        {
            return null;
        }

        if (!Enum.TryParse<DiscountType>(
                typeText,
                true,
                out var discountType))
        {
            return null;
        }

        var promoCode = new PromoCode(
            code,
            discountType,
            value,
            validFrom,
            validUntil);

        if (!isActive)
            promoCode.Deactivate();

        return promoCode;
    }

    private async Task SaveAllAsync(
        List<PromoCode> promoCodes)
    {
        var lines = promoCodes.Select(
            x => string.Join(
                " | ",
                x.Code,
                x.DiscountType,
                x.Value.ToString(
                    CultureInfo.InvariantCulture),
                x.ValidFrom.ToString("O"),
                x.ValidUntil.ToString("O"),
                x.IsActive));

        await _fileManager.WriteAllLinesAsync(
            FileName,
            lines);
    }
}