using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Offer;

public class UpdateOfferDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public IFormFile? Image { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public decimal? BundlePrice { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}