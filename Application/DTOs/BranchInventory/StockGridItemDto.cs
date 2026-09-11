using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.BranchInventory
{
    public class StockGridItemDto
    {
        public int ProductId { get; set; }
        public int BranchId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public string SalesRate { get; set; } = "0/wk";
        public int CoverageDays { get; set; }
    }
}
