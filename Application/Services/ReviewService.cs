using Application.DTOs.Review;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Helpers;

namespace Application.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IProductRepository _productRepository;

    public ReviewService(
        IReviewRepository reviewRepository,
        IProductRepository productRepository)
    {
        _reviewRepository = reviewRepository;
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ReviewDto>> GetByProductAsync(int productId)
    {
        var reviews = await _reviewRepository.GetByProductAsync(productId);
        return reviews.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<ReviewDto>> GetMyReviewsAsync(int customerId)
    {
        var reviews = await _reviewRepository.GetByCustomerAsync(customerId);
        return reviews.Select(MapToDto).ToList();
    }

    public async Task<ReviewDto> CreateAsync(int customerId, CreateReviewDto dto)
    {
        if (!await _productRepository.ExistsAsync(dto.ProductId))
            throw new Exception("المنتج مش موجود.");

        if (await _reviewRepository.CustomerReviewedProductAsync(customerId, dto.ProductId))
            throw new Exception("انت عملت تقييم للمنتج ده قبل كده.");

        var review = new Review
        {
            CustomerId = customerId,
            ProductId = dto.ProductId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await _reviewRepository.AddAsync(review);

        var created = await _reviewRepository.GetByIdAsync(review.Id);
        return MapToDto(created!);
    }

    public async Task<bool> UpdateAsync(int reviewId, int customerId, UpdateReviewDto dto)
    {
        var review = await _reviewRepository.GetByIdAsync(reviewId);
        if (review == null) return false;

        if (review.CustomerId != customerId)
            throw new Exception("مش مسموحلك تعدل التقييم ده.");

        review.Rating = dto.Rating;
        review.Comment = dto.Comment;

        await _reviewRepository.UpdateAsync(review);
        return true;
    }

    public async Task<bool> DeleteAsync(int reviewId, int customerId)
    {
        var review = await _reviewRepository.GetByIdAsync(reviewId);
        if (review == null) return false;

        if (review.CustomerId != customerId)
            throw new Exception("مش مسموحلك تمسح التقييم ده.");

        await _reviewRepository.DeleteAsync(review);
        return true;
    }
    private static ReviewDto MapToDto(Review r) => new()
    {
        Id = r.Id,
        CustomerName = $"{r.Customer?.FirstName} {r.Customer?.LastName}".Trim(),
        ProductId = r.ProductId,
        ProductName = r.Product?.Name ?? string.Empty,
        Rating = r.Rating,
        Comment = r.Comment,
        CreatedAt = r.CreatedAt.ToEgyptTime(),
        CustomerId = r.CustomerId
    };
}