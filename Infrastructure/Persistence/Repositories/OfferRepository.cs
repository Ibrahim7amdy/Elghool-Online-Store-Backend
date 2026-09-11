using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class OfferRepository : IOfferRepository
{
    private readonly AppDbContext _context;

    public OfferRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Offer>> GetAllAsync()
        => await _context.Offers
            .Include(o => o.OfferProducts)
                .ThenInclude(op => op.Product)
                    .ThenInclude(p => p.Images)
            .ToListAsync();

    public async Task<IEnumerable<Offer>> GetActiveAsync()
        => await _context.Offers
            .Include(o => o.OfferProducts)
                .ThenInclude(op => op.Product)
                    .ThenInclude(p => p.Images)
            .Where(o => o.IsActive && o.StartDate <= DateTime.UtcNow && o.EndDate >= DateTime.UtcNow)
            .ToListAsync();

    public async Task<Offer?> GetByIdAsync(int id)
        => await _context.Offers
            .Include(o => o.OfferProducts)
                .ThenInclude(op => op.Product)
                    .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(o => o.Id == id);

    public async Task AddAsync(Offer offer)
    {
        await _context.Offers.AddAsync(offer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Offer offer)
    {
        _context.Offers.Update(offer);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Offer offer)
    {
        _context.Offers.Remove(offer);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
        => await _context.Offers.AnyAsync(o => o.Id == id);



    public async Task<int> GetOrdersCountByOfferAsync(int offerId)
    => await _context.Orders.CountAsync(o => o.OfferId == offerId);

    public async Task<Dictionary<int, int>> GetOrdersCountForAllOffersAsync()
        => await _context.Orders
            .Where(o => o.OfferId != null)
            .GroupBy(o => o.OfferId!.Value)
            .Select(g => new { OfferId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.OfferId, x => x.Count);


    public async Task<Offer?> GetByIdWithProductsAsync(int offerId)
    {
        return await _context.Offers
            .Include(o => o.OfferProducts)
                .ThenInclude(op => op.Product)
            .FirstOrDefaultAsync(o => o.Id == offerId);
    }


    public async Task<Offer?> GetActivePercentageOfferForProductAsync(int productId)
    {
        var now = DateTime.UtcNow;

        return await _context.Offers
            .Where(o => o.IsActive
                     && o.StartDate <= now
                     && o.EndDate >= now
                     && o.DiscountPercentage.HasValue
                     && o.OfferProducts.Any(op => op.ProductId == productId))
            .FirstOrDefaultAsync();
    }
}