namespace Domain.Entities;

public class WishList
{
    public int CustomerId { get; set; }
    public int ProductId { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public Customer Customer { get; set; } = null!;
    public Product Product { get; set; } = null!;
}