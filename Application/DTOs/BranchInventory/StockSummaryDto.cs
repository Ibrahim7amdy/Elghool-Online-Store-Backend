using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.BranchInventory
{
    public class StockSummaryDto
    {
        public int TotalItemsCount { get; set; }
        public decimal TotalStockValue { get; set; }
        public int ActiveBranchesCount { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
    }
}
