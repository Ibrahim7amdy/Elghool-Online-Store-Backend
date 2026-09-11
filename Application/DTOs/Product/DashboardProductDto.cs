using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Product
{
    public class DashboardProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public string BrandName { get; set; } = null!; 
        public decimal OldPrice { get; set; }         
        public decimal NewPrice { get; set; }        
        public decimal? DiscountPercentage { get; set; } 
        public double AverageRating { get; set; }    
        public int ReviewsCount { get; set; }        
        public bool IsActive { get; set; }           
    }
}
