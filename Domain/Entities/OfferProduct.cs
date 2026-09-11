namespace Domain.Entities;

public class OfferProduct
{
    public int OfferId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;

    public Offer Offer { get; set; } = null!;
    public Product Product { get; set; } = null!;
}