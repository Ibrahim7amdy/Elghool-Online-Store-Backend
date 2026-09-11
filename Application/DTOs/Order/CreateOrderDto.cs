using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Order
{
    public class CreateOrderDto
    {
        public int? BranchId { get; set; }
        public string? Notes { get; set; }
        public int? OfferId { get; set; }

        public List<CreateOrderItemDto> Items { get; set; } = new();
    }

    public class CreateOrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
