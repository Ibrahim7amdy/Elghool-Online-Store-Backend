using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Branch
{
    public class BranchStatsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public decimal CurrentMonthRevenue { get; set; }
        public decimal LastMonthRevenue { get; set; }
        public decimal? RevenueChangePercentage { get; set; }
        public int OrdersCount { get; set; }
        public int CustomersCount { get; set; }
        public decimal? RevenueTarget { get; set; }
        public decimal? TargetAchievementPercentage { get; set; }
        public bool IsOpenNow { get; set; }
    }
}
