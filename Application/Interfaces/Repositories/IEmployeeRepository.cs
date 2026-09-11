using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(int id);
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<IEnumerable<Employee>> GetByBranchAsync(int branchId);
    Task<IEnumerable<Employee>> GetByRoleAsync(Domain.Enums.EmployeeRole role);
    Task<IEnumerable<Employee>> GetOnShiftNowAsync();
    Task<IEnumerable<Employee>> SearchAsync(string keyword);
    Task<IEnumerable<Employee>> FuzzySearchAsync(string keyword);
    Task<string> GenerateEmployeeCodeAsync();
    Task AddAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task<int> GetOnLeaveCountAsync();
}