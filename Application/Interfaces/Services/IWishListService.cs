using Application.DTOs.WishList;

namespace Application.Interfaces.Services;

public interface IWishListService
{
    Task<IEnumerable<WishListDto>> GetMyWishListAsync(int customerId);
    Task<bool> AddToWishListAsync(int customerId, int productId);
    Task<bool> RemoveFromWishListAsync(int customerId, int productId);
}