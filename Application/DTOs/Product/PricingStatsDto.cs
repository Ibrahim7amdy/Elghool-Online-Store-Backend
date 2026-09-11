namespace Application.DTOs.Product;

public class PricingStatsDto
{
    public int TotalProducts { get; set; }
    public decimal AvgMarginPercentage { get; set; }
    public int PriceChangesToday { get; set; }
}