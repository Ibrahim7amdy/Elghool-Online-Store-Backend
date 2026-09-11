namespace Application.DTOs.Product;

public class ProductImageDto
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}