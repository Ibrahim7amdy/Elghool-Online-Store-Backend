using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Dashboard
{
    public class CategoryShareDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public int OrdersCount { get; set; }
    }
}
