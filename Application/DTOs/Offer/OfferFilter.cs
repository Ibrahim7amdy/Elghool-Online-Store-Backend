using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Offer
{
    public class OfferFilter
    {
        public string? SearchTerm { get; set; }
        public string? Status { get; set; } // Active, Ended, Stopped,Upcoming
        public string? OfferType { get; set; } // Percentage, Bundle
        public string? SortBy { get; set; } // PriceAsc, PriceDesc, RequestsAsc, RequestsDesc
    }
}
