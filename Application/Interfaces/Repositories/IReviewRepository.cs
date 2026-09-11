using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(int id);
    Task<IEnumerable<Review>> GetByProductAsync(int productId);
    Task<IEnumerable<Review>> GetByCustomerAsync(int customerId);
    Task<bool> CustomerReviewedProductAsync(int customerId, int productId);
    Task AddAsync(Review review);
    Task UpdateAsync(Review review);
    Task DeleteAsync(Review review);
}