using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Review;

public class UpdateReviewDto
{
    public decimal Rating { get; set; }
    public string? Comment { get; set; }
}