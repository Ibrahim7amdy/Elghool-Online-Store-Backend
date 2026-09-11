using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.BranchInventory
{
    public class StockFilterDto
    {
        public string? SearchTerm { get; set; }
        public int? BranchId { get; set; }
        public int? CategoryId { get; set; }
        public string? Status { get; set; } // InStock, LowStock, OutOfStock
    }
}
