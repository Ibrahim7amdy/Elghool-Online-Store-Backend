using Application.DTOs.Review;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    // GET: api/review/product/5
    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetByProduct(int productId)
    {
        try
        {
            var reviews = await _reviewService.GetByProductAsync(productId);
            return Ok(reviews);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/review/my
    [Authorize(Policy = "CustomerOnly")]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyReviews()
    {
        try
        {
            var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var reviews = await _reviewService.GetMyReviewsAsync(customerId);
            return Ok(reviews);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST: api/review
    [Authorize(Policy = "CustomerOnly")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReviewDto dto)
    {
        try
        {
            var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var review = await _reviewService.CreateAsync(customerId, dto);
            return Ok(review);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/review/5
    [Authorize(Policy = "CustomerOnly")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateReviewDto dto)
    {
        try
        {
            var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _reviewService.UpdateAsync(id, customerId, dto);
            return result
                ? Ok(new { message = "تم تحديث التقييم بنجاح." })
                : NotFound(new { message = "التقييم مش موجود." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE: api/review/5
    [Authorize(Policy = "CustomerOnly")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _reviewService.DeleteAsync(id, customerId);
            return result
                ? Ok(new { message = "تم حذف التقييم بنجاح." })
                : NotFound(new { message = "التقييم مش موجود." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}