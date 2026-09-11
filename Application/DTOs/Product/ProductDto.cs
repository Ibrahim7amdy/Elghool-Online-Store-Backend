using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Product
{
    public class ProductDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal OldPrice { get; set; }

        public decimal NewPrice { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public bool HasDiscount => DiscountPercentage.HasValue && DiscountPercentage > 0;

        public string UnitType { get; set; } 

        public string? Description { get; set; }

        public List<ProductImageDto> Images { get; set; } = new();

        public decimal? Weight { get; set; }
        public string? WeightUnit { get; set; }
        public string? CategoryName { get; set; }

        public string? BrandName { get; set; }

        public double AverageRating { get; set; }

        public int ReviewsCount { get; set; }
        public bool IsAvailable { get; set; }
    }
}
