namespace Application.DTOs.Product;

public class MarginByCategoryDto
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal AvgMarginPercentage { get; set; }
}

public class RevenueByPriceTierDto
{
    public string Tier { get; set; } = string.Empty; // Premium, Standard, Budget
    public decimal Percentage { get; set; }
}