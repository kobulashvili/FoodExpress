using FoodExpress.Service.Interfaces;
using FoodExpress.Domain.Interfaces.Repositories;
using FoodExpress.Domain.Entity;
namespace FoodExpress.Service.Services;

public class MenuService : IMenuService
{
    private readonly IMenuRepository _menuRepository;

    public MenuService(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<List<MenuItem>> GetAllAsync()
    {
        return await _menuRepository.GetAllAsync();
    }

    public async Task<MenuItem?> GetByIdAsync(string id)
    {
        return await _menuRepository.GetByIdAsync(id);
    }

    public async Task<List<MenuItem>> FilterAsync(
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? category = null,
        int? restaurantId = null)
    {
        var items =
            await _menuRepository.GetAllAsync();

        IEnumerable<MenuItem> query = items;

        if (minPrice.HasValue)
        {
            query = query.Where(x =>
                x.GetFinalPrice() >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(x =>
                x.GetFinalPrice() <= maxPrice.Value);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(x =>
                x.Category.Equals(
                    category,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (restaurantId.HasValue)
        {
            query = query.Where(x =>
                x.RestaurantId == restaurantId.Value);
        }

        return query.ToList();
    }

    public async Task AddAsync(MenuItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        await _menuRepository.AddAsync(item);
    }

    public async Task UpdateAsync(MenuItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        await _menuRepository.UpdateAsync(item);
    }

    public async Task DeleteAsync(string id)
    {
        var item =
            await _menuRepository.GetByIdAsync(id);

        if (item == null)
            throw new KeyNotFoundException(
                $"Menu item '{id}' was not found.");

        await _menuRepository.DeleteAsync(id);
    }
}