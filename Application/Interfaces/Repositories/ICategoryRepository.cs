using Application.DTOs.Category;
using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);

        Task<IEnumerable<Category>> GetFlatCategoriesAsync(bool? isMainCategory = null);
        Task<Category> AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(Category category);
        Task<bool> CategoryExistsAsync(int id);
    }
}

