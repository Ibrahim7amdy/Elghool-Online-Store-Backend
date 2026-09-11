using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class WishListRepository : IWishListRepository
{
    private readonly AppDbContext _context;

    public WishListRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WishList>> GetByCustomerAsync(int customerId)
        => await _context.WishLists
            .Include(w => w.Product)
                .ThenInclude(p => p.Images)
            .Include(w => w.Product)
                .ThenInclude(p => p.BranchInventories)
            .Where(w => w.CustomerId == customerId)
            .OrderByDescending(w => w.AddedAt)
            .ToListAsync();

    public async Task<bool> ExistsAsync(int customerId, int productId)
        => await _context.WishLists
            .AnyAsync(w => w.CustomerId == customerId && w.ProductId == productId);

    public async Task AddAsync(WishList wishList)
    {
        await _context.WishLists.AddAsync(wishList);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(int customerId, int productId)
    {
        var wishList = await _context.WishLists
            .FirstOrDefaultAsync(w => w.CustomerId == customerId && w.ProductId == productId);

        if (wishList != null)
        {
            _context.WishLists.Remove(wishList);
            await _context.SaveChangesAsync();
        }
    }
}