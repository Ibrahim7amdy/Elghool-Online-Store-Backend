using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities;

public class Customer
{
    public int Customer_Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? GoogleId { get; set; }
    public bool IsExternalLogin { get; set; } = false;
    public int? PreferredBranchId { get; set; }
    public bool IsActive { get; set; } = true;
    public Branch? PreferredBranch { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<WishList> WishLists { get; set; } = new List<WishList>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}