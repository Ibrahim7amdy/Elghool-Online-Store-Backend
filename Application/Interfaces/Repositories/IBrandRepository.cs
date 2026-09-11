using Domain.Entities;

namespace Application.Interfaces.Repository;

public interface IBrandRepository
{
    Task<IEnumerable<Brand>> GetAllBrandsAsync();
    Task<Brand?> GetBrandByIdAsync(int id);
    Task<Brand> AddBrandAsync(Brand brand);
    Task UpdateBrandAsync(Brand brand);
    Task DeleteBrandAsync(Brand brand);
}