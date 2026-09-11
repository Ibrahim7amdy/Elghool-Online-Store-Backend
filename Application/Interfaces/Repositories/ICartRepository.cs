using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ICartRepository
{
    Task<IEnumerable<CartItem>> GetByCustomerAsync(int customerId);
    Task<CartItem?> GetItemAsync(int customerId, int productId);
    Task AddAsync(CartItem item);
    Task UpdateAsync(CartItem item);
    Task RemoveAsync(CartItem item);
    Task ClearAsync(int customerId);
}