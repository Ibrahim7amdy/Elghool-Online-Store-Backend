using Application.DTOs.Cart;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[Route("api/cart")]
[ApiController]
[Authorize(Policy = "CustomerOnly")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    // GET: api/cart
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        try
        {
            var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var cart = await _cartService.GetCartAsync(customerId);
            return Ok(cart);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST: api/cart
    [HttpPost]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
    {
        try
        {
            var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _cartService.AddToCartAsync(customerId, dto);
            return Ok(new { message = "تم إضافة المنتج للسلة." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/cart/{productId}
    [HttpPut("{productId}")]
    public async Task<IActionResult> UpdateCartItem(int productId, [FromBody] UpdateCartItemDto dto)
    {
        try
        {
            var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _cartService.UpdateCartItemAsync(customerId, productId, dto);
            return result
                ? Ok(new { message = "تم تحديث الكمية." })
                : NotFound(new { message = "المنتج مش موجود في السلة." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE: api/cart/{productId}
    [HttpDelete("{productId}")]
    public async Task<IActionResult> RemoveFromCart(int productId)
    {
        try
        {
            var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _cartService.RemoveFromCartAsync(customerId, productId);
            return result
                ? Ok(new { message = "تم إزالة المنتج من السلة." })
                : NotFound(new { message = "المنتج مش موجود في السلة." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE: api/cart
    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        try
        {
            var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _cartService.ClearCartAsync(customerId);
            return Ok(new { message = "تم تصفية السلة." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}