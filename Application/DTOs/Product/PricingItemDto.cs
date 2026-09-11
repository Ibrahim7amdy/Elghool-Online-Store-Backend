namespace Application.DTOs.Product;

public class PricingItemDto
{
    public int ProductId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal MarginPercentage { get; set; }
    public string? Brand { get; set; }
    public DateTime? LastUpdated { get; set; }
}