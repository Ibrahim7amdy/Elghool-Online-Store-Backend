namespace Domain.Entities;

public class Branch
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public TimeOnly OpeningTime { get; set; }
    public TimeOnly ClosingTime { get; set; }
    public bool IsActive { get; set; } = true;
    public decimal? RevenueTarget { get; set; }
    public int? ManagerId { get; set; }
    public Employee? Manager { get; set; }

    // Navigation Properties
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<BranchInventory> BranchInventories { get; set; } = new List<BranchInventory>();
}