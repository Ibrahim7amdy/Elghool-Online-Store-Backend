using Domain.Enums;

namespace Application.DTOs.Employee;

public class CreateEmployeeDto
{
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public EmployeeRole Role { get; set; }
    public int BranchId { get; set; }
    public TimeOnly WorkStart { get; set; }
    public TimeOnly WorkEnd { get; set; }
}