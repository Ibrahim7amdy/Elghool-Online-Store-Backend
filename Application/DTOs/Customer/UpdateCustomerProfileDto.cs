namespace Application.DTOs.Customer;

public class UpdateCustomerProfileDto
{
    public string FirstName { get; set; } =null!;   
    public string? LastName { get; set; } 
    public string PhoneNumber { get; set; } = null!;
    public int? PreferredBranchId { get; set; }
}