namespace Domain.Entities;

public class Offer
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public decimal? BundlePrice { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } // not for soft delete -- to check offer active or ended

    public ICollection<OfferProduct> OfferProducts { get; set; } = new List<OfferProduct>();
}