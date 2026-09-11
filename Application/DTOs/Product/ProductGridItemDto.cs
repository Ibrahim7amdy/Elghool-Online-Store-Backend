using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Product
{
    public class ProductGridItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public decimal PriceBeforeDiscount { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public double Rating { get; set; }
        public int ReviewsCount { get; set; }
        public bool IsActive { get; set; }
    }
}
