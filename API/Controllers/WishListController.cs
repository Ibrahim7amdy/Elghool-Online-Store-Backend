using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[Route("api/wishlist")]
[ApiController]
[Authorize(Policy = "CustomerOnly")]
public class WishListController : ControllerBase
{
    private readonly IWishListService _wishListService;

    public WishListController(IWishListService wishListService)
    {
        _wishListService = wishListService;
    }

    // GET: api/wishlist
    [HttpGet]
    public async Task<IActionResult> GetMyWishList()
    {
        try
        {
            var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var items = await _wishListService.GetMyWishListAsync(customerId);
            return Ok(items);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST: api/wishlist/5
    [HttpPost("{productId}")]
    public async Task<IActionResult> Add(int productId)
    {
        try
        {
            var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _wishListService.AddToWishListAsync(customerId, productId);
            return Ok(new { message = "تم إضافة المنتج للمفضلة." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE: api/wishlist/5
    [HttpDelete("{productId}")]
    public async Task<IActionResult> Remove(int productId)
    {
        try
        {
            var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _wishListService.RemoveFromWishListAsync(customerId, productId);
            return result
                ? Ok(new { message = "تم إزالة المنتج من المفضلة." })
                : NotFound(new { message = "المنتج مش موجود في المفضلة." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

}