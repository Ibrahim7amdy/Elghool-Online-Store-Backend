// Application/Services/EmployeeService.cs
using Application.DTOs.Employee;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IBranchRepository _branchRepository;

    public EmployeeService(
        IEmployeeRepository employeeRepository,
        IBranchRepository branchRepository)
    {
        _employeeRepository = employeeRepository;
        _branchRepository = branchRepository;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
    {
        var employees = await _employeeRepository.GetAllAsync();
        return employees.Select(MapToDto).ToList();
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        return employee == null ? null : MapToDto(employee);
    }

    public async Task<IEnumerable<EmployeeDto>> GetByBranchAsync(int branchId)
    {
        var employees = await _employeeRepository.GetByBranchAsync(branchId);
        return employees.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<EmployeeDto>> GetByRoleAsync(EmployeeRole role)
    {
        var employees = await _employeeRepository.GetByRoleAsync(role);
        return employees.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<EmployeeDto>> GetOnShiftNowAsync()
    {
        var employees = await _employeeRepository.GetOnShiftNowAsync();
        return employees.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<EmployeeDto>> SearchAsync(string keyword)
    {
        var employees = await _employeeRepository.SearchAsync(keyword);

        if (!employees.Any())
            employees = await _employeeRepository.FuzzySearchAsync(keyword);

        return employees.Select(MapToDto).ToList();
    }

    public async Task<EmployeeStatsDto> GetStatsAsync()
    {
        var all = await _employeeRepository.GetAllAsync();
        var onShift = await _employeeRepository.GetOnShiftNowAsync();

        var avgTenure = all.Any()
            ? all.Average(e => (DateTime.UtcNow - e.HireDate).TotalDays / 365)
            : 0;

        return new EmployeeStatsDto
        {
            TotalEmployees = all.Count(),
            OnShiftNow = onShift.Count(),
            AvgTenureYears = Math.Round(avgTenure, 1)
        };
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
    {
        if (!await _branchRepository.ExistsAsync(dto.BranchId))
            throw new Exception("الفرع مش موجود.");

        var employee = new Employee
        {
            EmployeeCode = await _employeeRepository.GenerateEmployeeCodeAsync(),
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            Role = dto.Role,
            BranchId = dto.BranchId,
            WorkStart = dto.WorkStart,
            WorkEnd = dto.WorkEnd,
            HireDate = DateTime.UtcNow,
            Status = EmployeeStatus.Active,
            IsActive = true
        };

        await _employeeRepository.AddAsync(employee);

        var created = await _employeeRepository.GetByIdAsync(employee.Id);
        return MapToDto(created!);
    }

    public async Task<bool> UpdateAsync(int id, UpdateEmployeeDto dto)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null) return false;

        if (!await _branchRepository.ExistsAsync(dto.BranchId))
            throw new Exception("الفرع مش موجود.");

        employee.FullName = dto.FullName;
        employee.PhoneNumber = dto.PhoneNumber;
        employee.Email = dto.Email;
        employee.Role = dto.Role;
        employee.BranchId = dto.BranchId;
        employee.WorkStart = dto.WorkStart;
        employee.WorkEnd = dto.WorkEnd;
        employee.Status = dto.Status;

        await _employeeRepository.UpdateAsync(employee);
        return true;
    }

    public async Task<bool> ToggleStatusAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null) return false;

        employee.IsActive = !employee.IsActive;
        employee.Status=(employee.IsActive)?EmployeeStatus.Active:EmployeeStatus.Inactive;
        await _employeeRepository.UpdateAsync(employee);
        return true;
    }

    private static EmployeeDto MapToDto(Employee e)
    {
        var now = TimeOnly.FromDateTime(DateTime.Now);
        return new EmployeeDto
        {
            Id = e.Id,
            EmployeeCode = e.EmployeeCode,
            FullName = e.FullName,
            PhoneNumber = e.PhoneNumber,
            Email = e.Email,
            Role = e.Role.ToString(),
            BranchName = e.Branch?.Name ?? string.Empty,
            WorkStart = e.WorkStart,
            WorkEnd = e.WorkEnd,
            Status = e.Status.ToString(),
            IsOnShift = (e.IsActive)?now >= e.WorkStart && now <= e.WorkEnd:false,
            TenureYears = Math.Round((DateTime.UtcNow - e.HireDate).TotalDays / 365, 1),
            IsActive = e.IsActive
        };
    }
}