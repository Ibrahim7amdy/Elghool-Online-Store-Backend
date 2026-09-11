using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class StockTransactionRepository : IStockTransactionRepository
{
    private readonly AppDbContext _context;

    public StockTransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(StockTransaction transaction)
    {
        await _context.StockTransactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task<List<StockTransaction>> GetHistoryAsync(int? productId, int? branchId)
    {
        var query = _context.StockTransactions
            .Include(st => st.Product)
            .Include(st => st.SourceBranch)
            .Include(st => st.DestinationBranch)
            .AsQueryable();

        if (productId.HasValue)
        {
            query = query.Where(st => st.ProductId == productId.Value);
        }

        if (branchId.HasValue)
        {
            query = query.Where(st =>
                st.SourceBranchId == branchId.Value ||
                st.DestinationBranchId == branchId.Value);
        }

        return await query
            .OrderByDescending(st => st.TransactionDate)
            .ToListAsync();
    }
}