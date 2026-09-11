using Application.DTOs.BranchInventory;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Helpers;

namespace Application.Services;

public class BranchInventoryService : IBranchInventoryService
{
    private readonly IBranchInventoryRepository _inventoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IStockTransactionRepository _transactionRepository;
    private readonly IOrderRepository _orderRepository;

    public BranchInventoryService(
        IBranchInventoryRepository inventoryRepository,
        IProductRepository productRepository,
        IBranchRepository branchRepository,
        IStockTransactionRepository transactionRepository,
        IOrderRepository orderRepository)
    {
        _inventoryRepository = inventoryRepository;
        _productRepository = productRepository;
        _branchRepository = branchRepository;
        _transactionRepository = transactionRepository;
        _orderRepository = orderRepository;

    }

    public async Task<StockSummaryDto> GetStockSummaryAsync()
    {
        var inventories = await _inventoryRepository.GetAllForSummaryAsync();
        var activeBranchesCount = await _branchRepository.GetActiveBranchesCountAsync();
        return new StockSummaryDto
        {
            TotalItemsCount = inventories.Where(bi => bi.Quantity > 0).Select(bi => bi.ProductId).Distinct().Count(),
            TotalStockValue = inventories.Sum(bi => bi.Quantity * (bi.Product?.Price ?? 0)),
            ActiveBranchesCount = activeBranchesCount,
            LowStockCount = inventories.Count(bi => bi.Quantity > 0 && bi.Quantity <= bi.LowStockThreshold),
            OutOfStockCount = inventories.Count(bi => bi.Quantity == 0)
        };
    }

    public async Task<List<StockGridItemDto>> GetStockGridAsync(StockFilterDto filter)
    {

        var list = await _inventoryRepository.GetFilteredAsync(filter.SearchTerm, filter.BranchId, filter.CategoryId, filter.Status);

        var lastWeek = DateTime.UtcNow.AddDays(-7);
        var recentSales = await _orderRepository.GetSalesSinceAsync(lastWeek);

        var result = list.Select(bi => MapToGridItemDto(bi, recentSales)).ToList();

        return result;
    }


    public async Task<bool> AddInventoryAsync(AddInventoryDto dto)
    {
        if (!await _productRepository.ExistsAsync(dto.ProductId)) return false;
        if (!await _branchRepository.ExistsAsync(dto.BranchId)) return false;
        if (await _inventoryRepository.ExistsAsync(dto.ProductId, dto.BranchId)) return false;

        var inventory = new BranchInventory
        {
            BranchId = dto.BranchId,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            LowStockThreshold = dto.LowStockThreshold ?? 10
        };

        await _inventoryRepository.AddAsync(inventory);

        var transaction = new StockTransaction
        {
            ProductId = dto.ProductId,
            DestinationBranchId = dto.BranchId,
            Quantity = dto.Quantity,
            Type = TransactionType.Addition,
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? "إضافة مخزون أولي" : dto.Notes
        };
        await _transactionRepository.AddAsync(transaction);

        return true;
    }

    public async Task<bool> UpdateInventoryAsync(int productId, int branchId, UpdateInventoryDto dto)
    {
        var inventory = await _inventoryRepository.GetByProductAndBranchAsync(productId, branchId);
        if (inventory == null) return false;

        int diff = dto.Quantity - inventory.Quantity;

        inventory.Quantity = dto.Quantity;
        if (dto.LowStockThreshold.HasValue) inventory.LowStockThreshold = dto.LowStockThreshold.Value;

        await _inventoryRepository.UpdateAsync(inventory);

        if (diff != 0)
        {
            var transaction = new StockTransaction
            {
                ProductId = productId,
                SourceBranchId = diff < 0 ? branchId : null,
                DestinationBranchId = diff > 0 ? branchId : null,
                Quantity = Math.Abs(diff),
                Type = diff > 0 ? TransactionType.Addition : TransactionType.Subtraction,
                Notes = dto.Notes
            };
            await _transactionRepository.AddAsync(transaction);
        }

        return true;
    }

    public async Task<bool> TransferStockAsync(TransferStockDto dto)
    {
        var sourceInventory = await _inventoryRepository.GetByProductAndBranchAsync(dto.ProductId, dto.FromBranchId);
        if (sourceInventory == null || sourceInventory.Quantity < dto.Quantity) return false;

        var destInventory = await _inventoryRepository.GetByProductAndBranchAsync(dto.ProductId, dto.ToBranchId);

        sourceInventory.Quantity -= dto.Quantity;
        await _inventoryRepository.UpdateAsync(sourceInventory);

        if (destInventory == null)
        {
            destInventory = new BranchInventory { ProductId = dto.ProductId, BranchId = dto.ToBranchId, Quantity = dto.Quantity };
            await _inventoryRepository.AddAsync(destInventory);
        }
        else
        {
            destInventory.Quantity += dto.Quantity;
            await _inventoryRepository.UpdateAsync(destInventory);
        }

        var transaction = new StockTransaction
        {
            ProductId = dto.ProductId,
            SourceBranchId = dto.FromBranchId,
            DestinationBranchId = dto.ToBranchId,
            Quantity = dto.Quantity,
            Type = TransactionType.Transfer,
            Notes = dto.Notes
        };
        await _transactionRepository.AddAsync(transaction);

        return true;
    }

    public async Task<List<StockHistoryDto>> GetStockHistoryAsync(int productId, int? branchId)
    {
        var history = await _transactionRepository.GetHistoryAsync(productId, branchId);
        return history.Select(st => new StockHistoryDto
        {
            Id = st.Id,
            ProductName = st.Product?.Name ?? "N/A",
            SourceBranchName = st.SourceBranch?.Name,
            DestinationBranchName = st.DestinationBranch?.Name,
            Quantity = st.Quantity,
            Type = st.Type,
            TransactionDate = st.TransactionDate.ToEgyptTime(),
            Notes = st.Notes
        }).ToList();
    }


    public async Task<List<BranchStockStatusDto>> GetStockStatusChartAsync()
    {
        var inventories = await _inventoryRepository.GetAllForSummaryAsync();

        return inventories.GroupBy(bi => bi.Branch?.Name ?? "Unknown")
            .Select(g => new BranchStockStatusDto
            {
                BranchName = g.Key,
                InStockCount = g.Count(bi => bi.Quantity > bi.LowStockThreshold),
                LowStockCount = g.Count(bi => bi.Quantity > 0 && bi.Quantity <= bi.LowStockThreshold),
                OutOfStockCount = g.Count(bi => bi.Quantity == 0)
            }).ToList();
    }

    public async Task<List<BranchStockValueDto>> GetStockValueChartAsync()
    {
        var inventories = await _inventoryRepository.GetAllForSummaryAsync();

        return inventories.GroupBy(bi => bi.Branch?.Name ?? "Unknown")
            .Select(g => new BranchStockValueDto
            {
                BranchName = g.Key,
                TotalValue = g.Sum(bi => bi.Quantity * (bi.Product?.Price ?? 0))
            }).ToList();
    }




    private StockGridItemDto MapToGridItemDto(BranchInventory bi, List<OrderItem> recentSales)
    {
        var totalSalesLastMonth = recentSales
            .Where(oi => oi.ProductId == bi.ProductId
                      && oi.Order.BranchId == bi.BranchId
                      && oi.Order.Status != OrderStatus.Pending)
            .Sum(oi => oi.Quantity);

        decimal weeklySalesRateDecimal = totalSalesLastMonth / 4.28m;
        int weeklySalesRate = (int)Math.Ceiling(weeklySalesRateDecimal);

        decimal dailySalesRate = weeklySalesRate / 7m;

        int coverageDays = dailySalesRate > 0
            ? (int)Math.Ceiling(bi.Quantity / dailySalesRate)
            : 0;

        return new StockGridItemDto
        {
            ProductId = bi.ProductId,
            BranchId = bi.BranchId,
            ProductName = bi.Product?.Name ?? string.Empty,
            CategoryName = bi.Product?.Category?.Name ?? "N/A",
            BranchName = bi.Branch?.Name ?? string.Empty,
            Quantity = bi.Quantity,
            Status = bi.Quantity == 0 ? "Out of Stock"
                   : bi.Quantity <= bi.LowStockThreshold ? "Low Stock"
                   : "In Stock",
            SalesRate = $"{weeklySalesRate}/wk",
            CoverageDays = coverageDays
        };
    }
}