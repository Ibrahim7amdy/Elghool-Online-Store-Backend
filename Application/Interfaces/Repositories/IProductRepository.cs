using Application.DTOs.Product;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task<bool> ExistsAsync(int id);

    Task<ProductDashboardStatsDto> GetProductStatsAsync();
    Task<IEnumerable<ChartDataDto>> GetCategoryDistributionAsync();
    Task<IEnumerable<ChartDataDto>> GetPriceDistributionAsync();
    Task<IEnumerable<Product>> GetFilteredProductsAsync(ProductFilterDto filter);
    Task<bool> ToggleProductStatusAsync(int id);


    Task<List<Product>> GetForPricingAsync(PricingFilterDto filter);
    Task<PricingStatsDto> GetPricingStatsAsync();
    Task<List<MarginByCategoryDto>> GetMarginByCategoryAsync();
    Task<List<RevenueByPriceTierDto>> GetRevenueByPriceTierAsync();

}