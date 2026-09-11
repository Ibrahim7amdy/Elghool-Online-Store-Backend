using Application.DTOs.Cart;

namespace Application.Interfaces.Services;

public interface ICartService
{
    Task<CartDto> GetCartAsync(int customerId);
    Task<bool> AddToCartAsync(int customerId, AddToCartDto dto);
    Task<bool> UpdateCartItemAsync(int customerId, int productId, UpdateCartItemDto dto);
    Task<bool> RemoveFromCartAsync(int customerId, int productId);
    Task ClearCartAsync(int customerId);
}