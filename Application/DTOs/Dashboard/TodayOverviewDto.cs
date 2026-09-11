using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Dashboard
{
    public class TodayOverviewDto
    {
        public string OpenBranches { get; set; } = string.Empty; // "11/11"
        public int ItemsSold { get; set; }
        public int StockAlertsCount { get; set; }
        public decimal TodayRevenue { get; set; }
        public decimal? TodayRevenueChangePercentage { get; set; } // مقارنة بالنهاردة اللي فات
    }

}
