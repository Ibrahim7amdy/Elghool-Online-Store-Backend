using Application.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IOrderService
    {
        // Customer
        Task<OrderDto> CreateOrderAsync(int customerId, CreateOrderDto dto);
        Task<IEnumerable<OrderDto>> GetMyOrdersAsync(int customerId);
        Task<OrderDto?> GetOrderByIdAsync(int orderId, int customerId);
        Task<bool> CancelOrderAsync(int orderId, int customerId);

        // Admin
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<IEnumerable<OrderDto>> GetOrdersByBranchAsync(int branchId);
        Task<OrderDto?> GetOrderByIdAdminAsync(int orderId);
        Task<bool> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto);
        Task<bool> CancelOrderByAdminAsync(int orderId);
    }
}
