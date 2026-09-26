using FoodExpress.Service.Interfaces;
using FoodExpress.Domain.Interfaces.Repositories;
using FoodExpress.Domain.Entity;
namespace FoodExpress.Service.Services;

public class PromoCodeService : IPromoCodeService
{
    private readonly IPromoCodeRepository _repository;

    public PromoCodeService(
        IPromoCodeRepository repository)
    {
        _repository = repository;
    }

    public async Task<PromoCode?> GetByCodeAsync(
        string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Promo code is required.");

        return await _repository.GetByCodeAsync(code);
    }

    public async Task<List<PromoCode>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task AddAsync(PromoCode promoCode)
    {
        if (promoCode == null)
            throw new ArgumentNullException(
                nameof(promoCode));

        var existing =
            await _repository.GetByCodeAsync(
                promoCode.Code);

        if (existing != null)
            throw new InvalidOperationException(
                "Promo code already exists.");

        await _repository.AddAsync(promoCode);
    }

    public async Task UpdateAsync(PromoCode promoCode)
    {
        if (promoCode == null)
            throw new ArgumentNullException(
                nameof(promoCode));

        var existing =
            await _repository.GetByCodeAsync(
                promoCode.Code);

        if (existing == null)
            throw new KeyNotFoundException(
                "Promo code was not found.");

        await _repository.UpdateAsync(promoCode);
    }

    public async Task DeleteAsync(string code)
    {
        var promoCode =
            await _repository.GetByCodeAsync(code);

        if (promoCode == null)
            throw new KeyNotFoundException(
                "Promo code was not found.");

        await _repository.DeleteAsync(code);
    }

    public async Task<decimal> CalculateDiscountAsync(
        string code,
        decimal amount)
    {
        if (amount <= 0)
            return 0;

        var promoCode =
            await _repository.GetByCodeAsync(code);

        if (promoCode == null)
            throw new KeyNotFoundException(
                "Promo code was not found.");

        if (!promoCode.IsValid(DateTime.Now))
            throw new InvalidOperationException(
                "Promo code is expired or inactive.");

        return promoCode.CalculateDiscount(amount);
    }
}