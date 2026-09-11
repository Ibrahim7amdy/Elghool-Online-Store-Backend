namespace Application.DTOs.Employee;

public class EmployeeDto
{
    public int Id { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string Role { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public TimeOnly WorkStart { get; set; }
    public TimeOnly WorkEnd { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsOnShift { get; set; }
    public double TenureYears { get; set; }
    public bool IsActive { get; set; }
}