namespace Application.DTOs.Customer;

public class CustomerStatsDto
{
    public int TotalCustomers { get; set; }
    public int NewThisMonth { get; set; }
    public decimal NewThisMonthPercentage { get; set; }
    public decimal AvgSpendPerCustomer { get; set; }
    public int ActiveToday { get; set; }
    public List<MonthlyGrowthDto> MonthlyGrowth { get; set; } = new();
    public CustomerTierStatsDto TierStats { get; set; } = new();
}

public class MonthlyGrowthDto
{
    public string Month { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class CustomerTierStatsDto
{
    public int VipCount { get; set; }      // > 500
    public int RegularCount { get; set; }  // 100-499
    public int OccasionalCount { get; set; } // < 100
}