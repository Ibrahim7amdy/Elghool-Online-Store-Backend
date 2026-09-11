namespace Application.DTOs.Cart;

public class CartItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal FinalPrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }
    public string? ProductImage { get; set; }
    public bool IsAvailable { get; set; }
}