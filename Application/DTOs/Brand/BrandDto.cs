namespace Application.DTOs.Brand;

public class BrandDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ProductsCount { get; set; }
    public bool IsActive {  get; set; }
}