using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Product;

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public UnitType UnitType { get; set; }
    public decimal? Weight { get; set; }
    public WeightUnit? WeightUnit { get; set; }
    public int CategoryId { get; set; }
    public int? BrandId { get; set; }
    public List<IFormFile>? Images { get; set; }
}