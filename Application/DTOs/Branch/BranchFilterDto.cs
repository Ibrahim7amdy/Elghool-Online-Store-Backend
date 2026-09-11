namespace Application.DTOs.Branch;

public class BranchFilterDto
{
    public string? Search { get; set; }
    public bool? IsOpen { get; set; } // true = Open, false = Closed
    public string? SortBy { get; set; } // "revenue" or "orders"
    public string? SortDir { get; set; } // "asc" or "desc"
}