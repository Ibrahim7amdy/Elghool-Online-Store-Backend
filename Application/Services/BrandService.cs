using Application.DTOs.Brand;
using Application.DTOs.Product;
using Application.Interfaces.Repository;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Application.Services;

public class BrandService : IBrandService
{
    private readonly IBrandRepository _brandRepository;

    public BrandService (IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<IEnumerable<BrandDto>> GetAllBrandsAsync()
    {
        var brands = await _brandRepository.GetAllBrandsAsync();
        return brands.Select(MapToDto).ToList();
    }

    public async Task<BrandDto?> GetBrandByIdAsync(int id)
    {
        var brand = await _brandRepository.GetBrandByIdAsync(id);
        return brand == null ? null : MapToDto(brand);
    }


    public async Task<BrandDto> CreateBrandAsync(CreateBrandDto dto)
    {

        var brand = new Brand
        {
            Name = dto.Name,
            IsActive = true
        };

        var created = await _brandRepository.AddBrandAsync(brand);
        return MapToDto(created);
    }

    public async Task<bool> UpdateBrandAsync(UpdateBrandDto dto)
    {
        var brand = await _brandRepository.GetBrandByIdAsync(dto.Id);
        if (brand == null) return false;


        brand.Name = dto.Name;
        brand.IsActive = dto.IsActive ?? brand.IsActive;

        await _brandRepository.UpdateBrandAsync(brand);
        return true;
    }

    public async Task<bool> DeleteBrandAsync(int id)
    {
        var brand = await _brandRepository.GetBrandByIdAsync(id);
        if (brand == null) return false;

        await _brandRepository.DeleteBrandAsync(brand);
        return true;
    }

    private BrandDto MapToDto(Brand brand) => new()
    {
        Id = brand.Id,
        Name = brand.Name,
        IsActive= brand.IsActive,
        ProductsCount = brand.IsActive? brand.Products?.Count ?? 0 : 0
    };
}