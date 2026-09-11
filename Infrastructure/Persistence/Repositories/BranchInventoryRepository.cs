using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class BranchInventoryRepository : IBranchInventoryRepository
{
    private readonly AppDbContext _context;

    public BranchInventoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<BranchInventory?> GetByProductAndBranchAsync(int productId, int branchId)
        => await _context.BranchInventories
            .Include(bi => bi.Branch)
            .Include(bi => bi.Product)
            .FirstOrDefaultAsync(bi => bi.ProductId == productId && bi.BranchId == branchId);
    public async Task<List<BranchInventory>> GetAllForSummaryAsync()
        => await _context.BranchInventories
            .Include(bi => bi.Product)
            .Include(bi => bi.Branch)
            .ToListAsync();

    public async Task<List<BranchInventory>> GetFilteredAsync(string? searchTerm, int? branchId, int? categoryId, string? status)
    {
        var query = _context.BranchInventories
            .Include(bi => bi.Product)
                .ThenInclude(p => p.Category)
            .Include(bi => bi.Branch)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(bi => bi.Product.Name.Contains(searchTerm));

        if (branchId.HasValue)
            query = query.Where(bi => bi.BranchId == branchId.Value);

        if (categoryId.HasValue)
            query = query.Where(bi => bi.Product.CategoryId == categoryId.Value);


        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Replace(" ", "");
            if (normalizedStatus.Equals("OutofStock", StringComparison.OrdinalIgnoreCase))
                query = query.Where(bi => bi.Quantity == 0);

            else if (normalizedStatus.Equals("LowStock", StringComparison.OrdinalIgnoreCase))
                query = query.Where(bi => bi.Quantity > 0 && bi.Quantity <= bi.LowStockThreshold);

            else if (normalizedStatus.Equals("InStock", StringComparison.OrdinalIgnoreCase))
                query = query.Where(bi => bi.Quantity > bi.LowStockThreshold);
        }

        return await query.ToListAsync();
    }

    public async Task AddAsync(BranchInventory inventory)
    {
        await _context.BranchInventories.AddAsync(inventory);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(BranchInventory inventory)
    {
        _context.BranchInventories.Update(inventory);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int productId, int branchId)
        => await _context.BranchInventories
            .AnyAsync(bi => bi.ProductId == productId && bi.BranchId == branchId);


    public IQueryable<BranchInventory> GetQueryable()
    {
        return _context.BranchInventories
            .Include(bi => bi.Product)
                .ThenInclude(p => p.Category)
            .Include(bi => bi.Branch)
            .AsQueryable();
    }
}