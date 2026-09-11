using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Auth;

public class CustomerRegisterDto
{
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int? PreferredBranchId { get; set; }
}