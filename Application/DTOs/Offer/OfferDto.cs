namespace Application.DTOs.Offer;

public class OfferDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public decimal? BundlePrice { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public string Status { get; set; } = string.Empty; // Active, Ended, Stopped,upcoming
    public int RequestsCount { get; set; }
    public int ProductsCount => Products?.Count ?? 0;
    public decimal TotalOfferPrice { get; set; }
    public List<OfferProductDto> Products { get; set; } = new();
}
public class OfferProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImage { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal FinalPrice { get; set; }
    public int Quantity { get; set; }
}
