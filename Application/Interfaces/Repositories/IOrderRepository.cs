using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int id);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<IEnumerable<Order>> GetByCustomerAsync(int customerId);
        Task<IEnumerable<Order>> GetByBranchAsync(int branchId);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task<string> GenerateOrderNumberAsync();
        Task DeleteAsync(Order order);
        Task<List<OrderItem>> GetSalesSinceAsync(DateTime sinceDate);
        Task<List<Order>> GetOrdersInRangeAsync(DateTime startUtc, DateTime endUtc);

    }

}
