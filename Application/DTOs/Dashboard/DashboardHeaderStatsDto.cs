using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Dashboard
{
    public class DashboardHeaderStatsDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal? RevenueChangePercentage { get; set; }

        public int TotalOrders { get; set; }
        public decimal? OrdersChangePercentage { get; set; }

        public int ActiveCustomers { get; set; }
        public decimal? CustomersChangePercentage { get; set; }

        public decimal AvgOrderValue { get; set; }
        public decimal? AvgOrderValueChangePercentage { get; set; }
    }

}
