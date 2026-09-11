using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _context;

    public ReviewRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Review?> GetByIdAsync(int id)
        => await _context.Reviews
            .Include(r => r.Customer)
            .Include(r => r.Product)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<IEnumerable<Review>> GetByProductAsync(int productId)
        => await _context.Reviews
            .Include(r => r.Customer)
            .Include(r => r.Product)
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<Review>> GetByCustomerAsync(int customerId)
        => await _context.Reviews
            .Include(r => r.Customer)
            .Include(r => r.Product)
            .Where(r => r.CustomerId == customerId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<bool> CustomerReviewedProductAsync(int customerId, int productId)
        => await _context.Reviews
            .AnyAsync(r => r.CustomerId == customerId && r.ProductId == productId);

    public async Task AddAsync(Review review)
    {
        await _context.Reviews.AddAsync(review);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Review review)
    {
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Review review)
    {
        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
    }
}