using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IBranchInventoryRepository
{
    Task<BranchInventory?> GetByProductAndBranchAsync(int productId, int branchId);
    Task AddAsync(BranchInventory inventory);
    Task UpdateAsync(BranchInventory inventory);
    Task<bool> ExistsAsync(int productId, int branchId);
    Task<List<BranchInventory>> GetAllForSummaryAsync();
    Task<List<BranchInventory>> GetFilteredAsync(string? searchTerm, int? branchId, int? categoryId,string? status);
}