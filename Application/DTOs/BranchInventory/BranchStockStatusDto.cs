using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.BranchInventory
{
    public class BranchStockStatusDto
    {
        public string BranchName { get; set; } = string.Empty;
        public int InStockCount { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
    }
}
