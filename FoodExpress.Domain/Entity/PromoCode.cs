using FoodExpress.Domain.Enums;

namespace FoodExpress.Domain.Entity;

public class PromoCode
{
    public string Code { get; private set; }
    public DiscountType DiscountType { get; private set; }
    public decimal Value { get; private set; }
    public DateTime ValidFrom { get; private set; }
    public DateTime ValidUntil { get; private set; }
    public bool IsActive { get; private set; }

    public PromoCode(
        string code,
        DiscountType discountType,
        decimal value,
        DateTime validFrom,
        DateTime validUntil)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Promo code is required.");

        if (value <= 0)
            throw new ArgumentException(
                "Discount value must be greater than zero.");

        if (validUntil <= validFrom)
            throw new ArgumentException(
                "Invalid promo code period.");

        if (discountType == DiscountType.Percentage &&
            value > 100)
            throw new ArgumentException(
                "Percentage discount cannot exceed 100.");

        Code = code;
        DiscountType = discountType;
        Value = value;
        ValidFrom = validFrom;
        ValidUntil = validUntil;
        IsActive = true;
    }

    public bool IsValid(DateTime date)
    {
        return IsActive &&
               date >= ValidFrom &&
               date <= ValidUntil;
    }

    public decimal CalculateDiscount(decimal amount)
    {
        if (amount <= 0)
            return 0;

        if (DiscountType == DiscountType.Percentage)
            return amount * Value / 100;

        return Math.Min(Value, amount);
    }

    public void Deactivate()
    {
        IsActive = false;
    }



}