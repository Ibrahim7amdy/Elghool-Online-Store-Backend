using Application.DTOs.Review;

namespace Application.Interfaces.Services;

public interface IReviewService
{
    Task<IEnumerable<ReviewDto>> GetByProductAsync(int productId);
    Task<IEnumerable<ReviewDto>> GetMyReviewsAsync(int customerId);
    Task<ReviewDto> CreateAsync(int customerId, CreateReviewDto dto);
    Task<bool> UpdateAsync(int reviewId, int customerId, UpdateReviewDto dto);
    Task<bool> DeleteAsync(int reviewId, int customerId);
}