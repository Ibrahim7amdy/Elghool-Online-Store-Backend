using Application.DTOs.Customer;

namespace Application.Interfaces.Services;

public interface IAdminCustomerService
{
    Task<IEnumerable<AdminCustomerDto>> GetAllAsync();
    Task<AdminCustomerDto?> GetByIdAsync(int id);
    Task<IEnumerable<AdminCustomerDto>> GetByBranchAsync(int branchId);
    Task<IEnumerable<AdminCustomerDto>> GetTopSpendersAsync();
    Task<IEnumerable<AdminCustomerDto>> GetNewThisMonthAsync();
    Task<IEnumerable<AdminCustomerDto>> SearchAsync(string keyword);
    Task<CustomerStatsDto> GetStatsAsync();
    Task<bool> ToggleStatusAsync(int id);
}