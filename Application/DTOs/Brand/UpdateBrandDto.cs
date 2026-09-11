using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Brand;

public class UpdateBrandDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool? IsActive { get; set; }
}