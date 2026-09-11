using Application.DTOs.Branch;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IBranchRepository
{
    Task<Branch?> GetByIdAsync(int id);
    Task<IEnumerable<Branch>> GetAllAsync();
    Task AddAsync(Branch branch);
    Task UpdateAsync(Branch branch);
    Task<bool> ExistsAsync(int id);
    Task<IEnumerable<Order>> GetOrdersByBranchAsync(int branchId);
    Task<IEnumerable<Branch>> SearchAsync(string keyword);
    Task<IEnumerable<Branch>> FuzzySearchAsync(string keyword);
    Task<int> GetCountAsync();
    Task<int> GetActiveBranchesCountAsync();
}