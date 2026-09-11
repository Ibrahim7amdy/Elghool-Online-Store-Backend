using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;

    public CartRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CartItem>> GetByCustomerAsync(int customerId)
        => await _context.CartItems
            .Include(c => c.Product)
                .ThenInclude(p => p.Images)
            .Include(c => c.Product)
                .ThenInclude(p => p.BranchInventories)
            .Where(c => c.CustomerId == customerId)
            .ToListAsync();

    public async Task<CartItem?> GetItemAsync(int customerId, int productId)
        => await _context.CartItems
            .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.ProductId == productId);

    public async Task AddAsync(CartItem item)
    {
        await _context.CartItems.AddAsync(item);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CartItem item)
    {
        _context.CartItems.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(CartItem item)
    {
        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
    }

    public async Task ClearAsync(int customerId)
    {
        var items = await _context.CartItems
            .Where(c => c.CustomerId == customerId)
            .ToListAsync();

        _context.CartItems.RemoveRange(items);
        await _context.SaveChangesAsync();
    }
}