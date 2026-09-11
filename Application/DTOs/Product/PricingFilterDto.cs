namespace Application.DTOs.Product;

public class PricingFilterDto
{
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public bool? LowMargin { get; set; } // Margin < 20%
    public bool? RecentlyUpdated { get; set; } // آخر 7 أيام مثلًا
}