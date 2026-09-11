using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.BranchInventory
{
    public class BranchStockValueDto
    {
        public string BranchName { get; set; } = string.Empty;
        public decimal TotalValue { get; set; }
    }
}
