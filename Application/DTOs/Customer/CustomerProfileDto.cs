namespace Application.DTOs.Customer;

public class CustomerProfileDto
{
    public string FirstName { get; set; } = null!;
    public string? LastName { get; set; } 
    public string PhoneNumber { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int? PreferredBranchId { get; set; }
    public int CustomerId { get; set; }
}