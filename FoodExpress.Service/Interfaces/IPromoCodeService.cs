using FoodExpress.Domain.Entity;

namespace FoodExpress.Service.Interfaces;

public interface IPromoCodeService
{
    Task<PromoCode?> GetByCodeAsync(string code);

    Task<List<PromoCode>> GetAllAsync();

    Task AddAsync(PromoCode promoCode);

    Task UpdateAsync(PromoCode promoCode);

    Task DeleteAsync(string code);

    Task<decimal> CalculateDiscountAsync(
        string code,
        decimal amount);
}