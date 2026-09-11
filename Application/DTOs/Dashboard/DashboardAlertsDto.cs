using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Dashboard
{
    public class DashboardAlertsDto
    {
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public int BranchesNeedingAttentionCount { get; set; }
        public int EmployeesOnLeaveCount { get; set; }
        public int TotalAlertsCount { get; set; }
    }
}
