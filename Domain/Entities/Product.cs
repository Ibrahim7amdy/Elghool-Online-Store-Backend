using Domain.Enums;

namespace Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; } // This is the "Selling Price"
    public decimal PurchasePrice { get; set; } 
    public decimal? DiscountPercentage { get; set; } 
    public UnitType UnitType { get; set; }
    public decimal? Weight { get; set; }
    public WeightUnit? WeightUnit { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } // only for pricing changes

    // Foreign Keys
    public int CategoryId { get; set; }
    public int? BrandId { get; set; }

    // Navigation Properties
    public Category Category { get; set; } = null!;
    public Brand? Brand { get; set; }
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<BranchInventory> BranchInventories { get; set; } = new List<BranchInventory>();
    public ICollection<OfferProduct> OfferProducts { get; set; } = new List<OfferProduct>();
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}