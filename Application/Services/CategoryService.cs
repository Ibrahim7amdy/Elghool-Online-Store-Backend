using Application.DTOs.Category;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IFileService _fileService;

        public CategoryService(ICategoryRepository categoryRepository, IFileService fileService)
        {
            _categoryRepository = categoryRepository;
            _fileService = fileService;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            var mainCategories = categories.Where(c => c.ParentCategoryId == null);
            return mainCategories.Select(MapToDto).ToList();
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null) return null;
            return MapToDto(category);
        }


        public async Task<List<FlatCategoryDto>> GetFlatCategoriesAsync(bool? isMainCategory = null)
        {
            var categories = await _categoryRepository.GetFlatCategoriesAsync(isMainCategory);

            return categories.Select(c => new FlatCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                ParentCategoryId = c.ParentCategoryId
            }).ToList();
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            string imageUrl = string.Empty;

            if (dto.Image != null)
            {
                // حفظ الصورة في مجلد categories
                imageUrl = await _fileService.UploadFileAsync(dto.Image, "categories");
            }

            var category = new Category
            {
                Name = dto.Name,
                ImageUrl = imageUrl,
                ParentCategoryId = dto.ParentCategoryId
            };

            var createdCategory = await _categoryRepository.AddCategoryAsync(category);
            return MapToDto(createdCategory);
        }

        public async Task<bool> UpdateCategoryAsync(UpdateCategoryDto dto)
        {
            var existingCategory = await _categoryRepository.GetCategoryByIdAsync(dto.Id);
            if (existingCategory == null) return false;

            if (dto.ParentCategoryId == dto.Id)
                throw new ArgumentException("Category cannot be a parent of itself.");

            // لو اليوزر رفع صورة جديدة في التعديل
            if (dto.Image != null)
            {
               
                if (!string.IsNullOrEmpty(existingCategory.ImageUrl))
                {
                    await _fileService.DeleteFileAsync(existingCategory.ImageUrl);
                }

                // ارفع الصورة الجديدة في مجلد categories
                existingCategory.ImageUrl = await _fileService.UploadFileAsync(dto.Image, "categories");
            }

            existingCategory.Name = dto.Name;
            existingCategory.ParentCategoryId = dto.ParentCategoryId;

            await _categoryRepository.UpdateCategoryAsync(existingCategory);
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null) return false;

            if (category.SubCategories.Any())
                throw new InvalidOperationException("Cannot delete a category that has sub-categories.");

            
            if (!string.IsNullOrEmpty(category.ImageUrl))
            {
                await _fileService.DeleteFileAsync(category.ImageUrl);
            }

            await _categoryRepository.DeleteCategoryAsync(category);
            return true;
        }

        // Helper Method for Mapping
        private CategoryDto MapToDto(Category category)
        {
            var subCategoryDtos = category.SubCategories.Select(MapToDto).ToList();

            var ownProductsCount = category.Products?.Count(p => p.IsActive) ?? 0;
            var subCategoriesProductsCount = subCategoryDtos.Sum(sc => sc.ProductsCount);

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = category.ImageUrl,
                ParentCategoryId = category.ParentCategoryId,
                ProductsCount = ownProductsCount + subCategoriesProductsCount,
                SubCategories = subCategoryDtos
            };
        }
    }
}