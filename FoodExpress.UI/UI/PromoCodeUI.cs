using FoodExpress.Domain.Entity;
using FoodExpress.Domain.Enums;
using FoodExpress.Service.Interfaces;
using FoodExpress.UI.Helpers;

namespace FoodExpress.UI.UI;

public class PromoCodeUI
{
    private readonly IPromoCodeService _promoCodeService;

    public PromoCodeUI(
        IPromoCodeService promoCodeService)
    {
        _promoCodeService = promoCodeService;
    }

    public async Task ShowAsync()
    {
        while (true)
        {
            ConsoleHelper.ShowTitle(
                "Promo Code Management");

            Console.WriteLine("1. View Promo Codes");
            Console.WriteLine("2. Add Promo Code");
            Console.WriteLine("3. Update Promo Code");
            Console.WriteLine("4. Delete Promo Code");
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
        var promoCodes =
            await _promoCodeService.GetAllAsync();

        if (promoCodes.Count == 0)
        {
            Console.WriteLine(
                "No promo codes found.");

            ConsoleHelper.Pause();
            return;
        }

        foreach (var promo in promoCodes)
        {
            Console.WriteLine(
                $"Code: {promo.Code}");

            Console.WriteLine(
                $"Type: {promo.DiscountType}");

            Console.WriteLine(
                $"Value: {promo.Value}");

            Console.WriteLine(
                $"Valid from: {promo.ValidFrom:yyyy-MM-dd HH:mm}");

            Console.WriteLine(
                $"Valid until: {promo.ValidUntil:yyyy-MM-dd HH:mm}");

            Console.WriteLine(
                $"Active: {promo.IsActive}");

            Console.WriteLine(
                "----------------------------------------");
        }

        ConsoleHelper.Pause();
    }

    private async Task AddAsync()
    {
        ConsoleHelper.ShowTitle(
            "Add Promo Code");

        var code =
            InputHelper.ReadString("Code: ");

        var type =
            ReadDiscountType();

        var value =
            InputHelper.ReadPositiveDecimal(
                "Value: ");

        var validFrom =
            ReadDateTime("Valid from");

        var validUntil =
            ReadDateTime("Valid until");

        var promoCode =
            new PromoCode(
                code,
                type,
                value,
                validFrom,
                validUntil);

        await _promoCodeService.AddAsync(
            promoCode);

        ConsoleHelper.ShowSuccess(
            "Promo code added successfully.");

        ConsoleHelper.Pause();
    }

    private async Task UpdateAsync()
    {
        ConsoleHelper.ShowTitle(
            "Update Promo Code");

        var code =
            InputHelper.ReadString("Code: ");

        var existing =
            await _promoCodeService
                .GetByCodeAsync(code);

        if (existing == null)
            throw new KeyNotFoundException(
                "Promo code was not found.");

        var type =
            ReadDiscountType();

        var value =
            InputHelper.ReadPositiveDecimal(
                "Value: ");

        var validFrom =
            ReadDateTime("Valid from");

        var validUntil =
            ReadDateTime("Valid until");

        var updated =
            new PromoCode(
                existing.Code,
                type,
                value,
                validFrom,
                validUntil);

        if (!existing.IsActive)
            updated.Deactivate();

        await _promoCodeService.UpdateAsync(
            updated);

        ConsoleHelper.ShowSuccess(
            "Promo code updated successfully.");

        ConsoleHelper.Pause();
    }

    private async Task DeleteAsync()
    {
        var code =
            InputHelper.ReadString("Code: ");

        await _promoCodeService.DeleteAsync(
            code);

        ConsoleHelper.ShowSuccess(
            "Promo code deleted successfully.");

        ConsoleHelper.Pause();
    }

    private static DiscountType ReadDiscountType()
    {
        while (true)
        {
            Console.WriteLine(
                "1. Percentage");

            Console.WriteLine(
                "2. Fixed");

            var choice =
                InputHelper.ReadString(
                    "Discount type: ");

            switch (choice)
            {
                case "1":
                    return DiscountType.Percentage;

                case "2":
                    return DiscountType.Fixed;

                default:
                    ConsoleHelper.ShowError(
                        "Invalid discount type.");
                    break;
            }
        }
    }

    private static DateTime ReadDateTime(
        string name)
    {
        while (true)
        {
            Console.Write(
                $"{name} (yyyy-MM-dd HH:mm): ");

            var input =
                Console.ReadLine();

            if (DateTime.TryParse(
                    input,
                    out var date))
            {
                return date;
            }

            Console.WriteLine(
                "Invalid date format.");
        }
    }
}