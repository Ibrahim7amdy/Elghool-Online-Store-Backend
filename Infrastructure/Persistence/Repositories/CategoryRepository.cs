using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context; 

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
            .AsNoTracking()
            .Where(c => c.ParentCategoryId == null)
            .Include(c => c.Products.Where(p => p.IsActive))
            .Include(c => c.SubCategories)
                .ThenInclude(sc => sc.Products.Where(p => p.IsActive))
            .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories
        .Include(c => c.Products.Where(p => p.IsActive))
        .Include(c => c.SubCategories)
            .ThenInclude(sc => sc.Products.Where(p => p.IsActive))
        .FirstOrDefaultAsync(c => c.Id == id);
        }


        public async Task<IEnumerable<Category>> GetFlatCategoriesAsync(bool? isMainCategory = null)
        {
            var query = _context.Categories.AsNoTracking().AsQueryable();

            if (isMainCategory.HasValue)
            {
                if (isMainCategory.Value) query = query.Where(c => c.ParentCategoryId == null);

                else query = query.Where(c => c.ParentCategoryId != null);

            }

            return await query.ToListAsync();
        }


        public async Task<Category> AddCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CategoryExistsAsync(int id)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id);
        }
    }
}