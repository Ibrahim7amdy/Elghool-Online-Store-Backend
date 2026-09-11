using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IWishListRepository
{
    Task<IEnumerable<WishList>> GetByCustomerAsync(int customerId);
    Task<bool> ExistsAsync(int customerId, int productId);
    Task AddAsync(WishList wishList);
    Task RemoveAsync(int customerId, int productId);
}