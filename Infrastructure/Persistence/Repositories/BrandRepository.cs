using Application.Interfaces.Repository;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class BrandRepository : IBrandRepository
{
    private readonly AppDbContext _context;

    public BrandRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Brand>> GetAllBrandsAsync()
     => await _context.Brands
         .Include(b => b.Products.Where(p => p.IsActive))
         .Where(b => b.IsActive)
         .ToListAsync();

    public async Task<Brand?> GetBrandByIdAsync(int id)
        => await _context.Brands
            .Include(b => b.Products.Where(p => p.IsActive))
            .FirstOrDefaultAsync(b => b.Id == id);
    public async Task<Brand> AddBrandAsync(Brand brand)
    {
        await _context.Brands.AddAsync(brand);
        await _context.SaveChangesAsync();
        return brand;
    }

    public async Task UpdateBrandAsync(Brand brand)
    {
        _context.Brands.Update(brand);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteBrandAsync(Brand brand)
    {
        brand.IsActive = false; // Soft Delete 
        await _context.SaveChangesAsync();
    }
}