namespace Application.DTOs.Employee;

public class EmployeeStatsDto
{
    public int TotalEmployees { get; set; }
    public int OnShiftNow { get; set; }
    public double AvgTenureYears { get; set; }
}