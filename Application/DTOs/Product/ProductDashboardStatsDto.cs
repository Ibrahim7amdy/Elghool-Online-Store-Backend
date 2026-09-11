namespace Application.DTOs.Product;

public class ProductDashboardStatsDto
{
    public int TotalProducts { get; set; }
    public int CategoriesCount { get; set; }
    public int BrandsCount { get; set; }
    public decimal AveragePrice { get; set; }
    public double AverageRating { get; set; }
}