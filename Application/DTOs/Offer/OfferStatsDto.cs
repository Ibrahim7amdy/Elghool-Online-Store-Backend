namespace Application.DTOs.Offer;

public class OfferStatsDto
{
    public int TotalOffers { get; set; }
    public int ActiveNow { get; set; }
    public decimal SuccessRate { get; set; }
    public int TotalProductsInOffers { get; set; }
    public string? MostRequestedOfferTitle { get; set; }
    public int MostRequestedCount { get; set; }
    public List<OfferTypeDistributionDto> TypeDistribution { get; set; } = new();
    public List<TopRequestedOfferDto> TopRequestedOffers { get; set; } = new();
}

public class OfferTypeDistributionDto
{
    public string Type { get; set; } = string.Empty; // Percentage, Bundle
    public int Count { get; set; }
}

public class TopRequestedOfferDto
{
    public string Title { get; set; } = string.Empty;
    public int RequestsCount { get; set; }
}