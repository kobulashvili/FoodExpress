using FoodExpress.Domain.Entity;

namespace FoodExpress.Domain.Interfaces.Repositories;

public interface IPromoCodeRepository
{
    Task<List<PromoCode>> GetAllAsync();

    Task<PromoCode?> GetByCodeAsync(string code);

    Task AddAsync(PromoCode promoCode);

    Task UpdateAsync(PromoCode promoCode);

    Task DeleteAsync(string code);
}