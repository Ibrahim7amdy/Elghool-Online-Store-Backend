namespace Application.DTOs.Dashboard;

public class DashboardOverviewDto
{
    public DashboardHeaderStatsDto HeaderStats { get; set; } = new();
    public List<RevenuePointDto> RevenueTrend { get; set; } = new();
    public List<CategoryShareDto> OrdersByCategory { get; set; } = new();
    public TodayOverviewDto TodayOverview { get; set; } = new();
    public DashboardAlertsDto Alerts { get; set; } = new();
}