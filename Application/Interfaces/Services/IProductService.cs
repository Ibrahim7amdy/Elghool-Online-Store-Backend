using Application.DTOs.Product;

namespace Application.Interfaces.Services;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<ProductDto> CreateProductAsync(CreateProductDto dto);
    Task<bool> UpdateProductAsync(int id, UpdateProductDto dto);

    Task<ProductDashboardStatsDto> GetProductStatsAsync();
    Task<IEnumerable<ChartDataDto>> GetCategoryDistributionAsync();
    Task<IEnumerable<ChartDataDto>> GetPriceDistributionAsync();
    Task<IEnumerable<ProductDto>> AppGetFilteredProductsAsync(ProductFilterDto filter);
    Task<List<DashboardProductDto>> DashboardGetFilteredProducts(ProductFilterDto filter);
    Task<bool> ToggleProductStatusAsync(int id);


    Task<List<PricingItemDto>> GetPricingTableAsync(PricingFilterDto filter);
    Task<PricingStatsDto> GetPricingStatsAsync();
    Task<List<MarginByCategoryDto>> GetMarginByCategoryChartAsync();
    Task<List<RevenueByPriceTierDto>> GetRevenueByPriceTierChartAsync();
    Task<bool> UpdateProductPriceAsync(int productId, UpdateSinglePriceDto dto);


}
