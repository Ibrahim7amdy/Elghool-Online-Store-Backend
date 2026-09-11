namespace Application.DTOs.Category
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int? ParentCategoryId { get; set; }
        public int ProductsCount { get; set; }
        public List<CategoryDto> SubCategories { get; set; } = new List<CategoryDto>();
    }


}