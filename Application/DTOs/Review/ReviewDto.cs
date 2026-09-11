using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Review;

public class ReviewDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}