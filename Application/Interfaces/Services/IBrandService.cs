using Application.DTOs.Brand;
using Application.DTOs.Product;

namespace Application.Interfaces.Services;

public interface IBrandService
{
    Task<IEnumerable<BrandDto>> GetAllBrandsAsync();
    Task<BrandDto?> GetBrandByIdAsync(int id);
    Task<BrandDto> CreateBrandAsync(CreateBrandDto dto);
    Task<bool> UpdateBrandAsync(UpdateBrandDto dto);
    Task<bool> DeleteBrandAsync(int id);
}