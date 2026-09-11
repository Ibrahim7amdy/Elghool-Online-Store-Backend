using Application.DTOs.Category;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(int id);

        Task<List<FlatCategoryDto>> GetFlatCategoriesAsync(bool? isMainCategory = null);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto);
        Task<bool> UpdateCategoryAsync(UpdateCategoryDto dto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
