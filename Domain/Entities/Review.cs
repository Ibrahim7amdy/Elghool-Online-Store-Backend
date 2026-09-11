using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public decimal Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Customer Customer { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
