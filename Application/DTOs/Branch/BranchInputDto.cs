using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Branch;

public class BranchInputDto
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public TimeOnly OpeningTime { get; set; }
    public TimeOnly ClosingTime { get; set; }
    public bool? IsActive { get; set; } = true;
    public decimal? RevenueTarget { get; set; }
}