namespace Application.DTOs.WishList;

public class WishListDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public bool HasDiscount => DiscountPercentage.HasValue && DiscountPercentage > 0;
    public string? ProductImage { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime AddedAt { get; set; }
}