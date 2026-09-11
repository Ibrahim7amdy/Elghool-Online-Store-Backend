using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    private IQueryable<Order> GetOrdersWithIncludes()
        => _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Branch)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.Images);

    public async Task<Order?> GetByIdAsync(int id)
        => await GetOrdersWithIncludes()
            .FirstOrDefaultAsync(o => o.Id == id);

    public async Task<IEnumerable<Order>> GetAllAsync()
        => await GetOrdersWithIncludes()
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<Order>> GetByCustomerAsync(int customerId)
        => await GetOrdersWithIncludes()
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<Order>> GetByBranchAsync(int branchId)
        => await GetOrdersWithIncludes()
            .Where(o => o.BranchId == branchId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }


    public async Task DeleteAsync(Order order)
    {
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
    }

    public async Task<string> GenerateOrderNumberAsync()
    {
        var egyptToday = DateTime.UtcNow.ToEgyptTime();
        var todayStr = egyptToday.ToString("yyyyMMdd");
        var count = await _context.Orders
            .CountAsync(o => o.CreatedAt.Date == DateTime.UtcNow.Date);
        return $"ELG-{todayStr}-{(count + 1):D4}";
    }

    public async Task<List<OrderItem>> GetSalesSinceAsync(DateTime sinceDate)
    {
        return await _context.OrderItems
            .Include(oi => oi.Order)
            .Where(oi => oi.Order.CreatedAt >= sinceDate)
            .ToListAsync();
    }


    public async Task<List<Order>> GetOrdersInRangeAsync(DateTime startUtc, DateTime endUtc)
    => await _context.Orders
        .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.Category)
        .Where(o => o.CreatedAt >= startUtc && o.CreatedAt < endUtc)
        .ToListAsync();

}