// Application/DTOs/Brand/CreateBrandDto.cs
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Brand;

public class CreateBrandDto
{
    public string Name { get; set; } = string.Empty;
}