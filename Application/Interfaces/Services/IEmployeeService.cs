using Application.DTOs.Employee;
using Domain.Enums;

namespace Application.Interfaces.Services;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllAsync();
    Task<EmployeeDto?> GetByIdAsync(int id);
    Task<IEnumerable<EmployeeDto>> GetByBranchAsync(int branchId);
    Task<IEnumerable<EmployeeDto>> GetByRoleAsync(EmployeeRole role);
    Task<IEnumerable<EmployeeDto>> GetOnShiftNowAsync();
    Task<IEnumerable<EmployeeDto>> SearchAsync(string keyword);
    Task<EmployeeStatsDto> GetStatsAsync();
    Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto);
    Task<bool> UpdateAsync(int id, UpdateEmployeeDto dto);
    Task<bool> ToggleStatusAsync(int id);
}