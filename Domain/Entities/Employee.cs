using Domain.Enums;

namespace Domain.Entities;

public class Employee
{
    public int Id { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public EmployeeRole Role { get; set; }
    public int BranchId { get; set; }
    public TimeOnly WorkStart { get; set; }
    public TimeOnly WorkEnd { get; set; }
    public DateTime HireDate { get; set; } = DateTime.UtcNow;
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
    public bool IsActive { get; set; } = true;

    public Branch Branch { get; set; } = null!;
}