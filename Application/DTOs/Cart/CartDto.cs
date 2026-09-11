namespace Application.DTOs.Cart;

public class CartDto
{
    public List<CartItemDto> Items { get; set; } = new();
    public decimal TotalPrice { get; set; }
    public int TotalItems { get; set; }
}