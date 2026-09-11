using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Review;

public class CreateReviewDto
{
    public int ProductId { get; set; }
    public decimal Rating { get; set; }
    public string? Comment { get; set; }
}