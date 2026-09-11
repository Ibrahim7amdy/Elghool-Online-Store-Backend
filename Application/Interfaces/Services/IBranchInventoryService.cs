using Application.DTOs.BranchInventory;

namespace Application.Interfaces.Services;

public interface IBranchInventoryService
{
    Task<bool> AddInventoryAsync(AddInventoryDto dto);
    Task<bool> UpdateInventoryAsync(int productId, int branchId, UpdateInventoryDto dto);

    Task<StockSummaryDto> GetStockSummaryAsync();
    Task<List<StockGridItemDto>> GetStockGridAsync(StockFilterDto filter);
    Task<bool> TransferStockAsync(TransferStockDto dto);
    Task<List<StockHistoryDto>> GetStockHistoryAsync(int productId, int? branchId);
    Task<List<BranchStockStatusDto>> GetStockStatusChartAsync();
    Task<List<BranchStockValueDto>> GetStockValueChartAsync();

}