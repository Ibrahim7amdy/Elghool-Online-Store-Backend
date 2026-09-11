using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.BranchInventory
{
    public class TransferStockDto
    {
        public int ProductId { get; set; }
        public int FromBranchId { get; set; }
        public int ToBranchId { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
