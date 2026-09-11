using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class StockTransaction
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int? SourceBranchId { get; set; }
        public Branch? SourceBranch { get; set; }

        public int? DestinationBranchId { get; set; }
        public Branch? DestinationBranch { get; set; }

        public int Quantity { get; set; }
        public TransactionType Type { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public string Notes { get; set; } = string.Empty;
    }
}
