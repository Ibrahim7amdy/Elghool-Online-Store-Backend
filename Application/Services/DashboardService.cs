using Application.DTOs.Dashboard;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Enums;
using Domain.Helpers;

namespace Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IBranchInventoryService _inventoryService;
    private readonly BranchService _branchService;

    public DashboardService(
        IOrderRepository orderRepository,
        IBranchRepository branchRepository,
        IEmployeeRepository employeeRepository,
        IBranchInventoryService inventoryService,
        BranchService branchService)
    {
        _orderRepository = orderRepository;
        _branchRepository = branchRepository;
        _employeeRepository = employeeRepository;
        _inventoryService = inventoryService;
        _branchService = branchService;
    }

    public async Task<DashboardOverviewDto> GetOverviewAsync(DashboardPeriod period)
    {
        var days = period switch
        {
            DashboardPeriod.SevenDays => 7,
            DashboardPeriod.ThirtyDays => 30,
            DashboardPeriod.NinetyDays => 90,
            _ => 7
        };

        var todayStartEgypt = DateTimeHelper.EgyptNow.Date;
        var todayStartUtc = todayStartEgypt.ToUtcFromEgyptTime();

        var currentPeriodStartUtc = todayStartUtc.AddDays(-days + 1);
        var previousPeriodStartUtc = currentPeriodStartUtc.AddDays(-days);
        var previousPeriodEndUtc = currentPeriodStartUtc;
        var endUtc = todayStartUtc.AddDays(1); 

        var currentOrders = await _orderRepository.GetOrdersInRangeAsync(currentPeriodStartUtc, endUtc);
        var previousOrders = await _orderRepository.GetOrdersInRangeAsync(previousPeriodStartUtc, previousPeriodEndUtc);

        return new DashboardOverviewDto
        {
            HeaderStats = BuildHeaderStats(currentOrders, previousOrders),
            RevenueTrend = BuildRevenueTrend(currentOrders, currentPeriodStartUtc, days),
            OrdersByCategory = BuildCategoryShare(currentOrders),
            TodayOverview = await BuildTodayOverviewAsync(todayStartUtc, endUtc),
            Alerts = await BuildAlertsAsync()
        };
    }

    private DashboardHeaderStatsDto BuildHeaderStats(List<Domain.Entities.Order> current, List<Domain.Entities.Order> previous)
    {
        var currentRevenue = current.Sum(o => o.TotalPrice);
        var previousRevenue = previous.Sum(o => o.TotalPrice);

        var currentOrdersCount = current.Count;
        var previousOrdersCount = previous.Count;

        var currentCustomers = current.Select(o => o.CustomerId).Distinct().Count();
        var previousCustomers = previous.Select(o => o.CustomerId).Distinct().Count();

        var currentAov = currentOrdersCount == 0 ? 0 : Math.Round(currentRevenue / currentOrdersCount, 2);
        var previousAov = previousOrdersCount == 0 ? 0 : Math.Round(previousRevenue / previousOrdersCount, 2);

        return new DashboardHeaderStatsDto
        {
            TotalRevenue = currentRevenue,
            RevenueChangePercentage = CalculateChange(currentRevenue, previousRevenue),

            TotalOrders = currentOrdersCount,
            OrdersChangePercentage = CalculateChange(currentOrdersCount, previousOrdersCount),

            ActiveCustomers = currentCustomers,
            CustomersChangePercentage = CalculateChange(currentCustomers, previousCustomers),

            AvgOrderValue = currentAov,
            AvgOrderValueChangePercentage = CalculateChange(currentAov, previousAov)
        };
    }



    private List<RevenuePointDto> BuildRevenueTrend(List<Domain.Entities.Order> orders, DateTime startUtc, int days)
    {
        var points = new List<RevenuePointDto>();
        for (int i = 0; i < days; i++)
        {
            var dayStartUtc = startUtc.AddDays(i);
            var dayEndUtc = dayStartUtc.AddDays(1);

            var dayRevenue = orders
                .Where(o => o.CreatedAt >= dayStartUtc && o.CreatedAt < dayEndUtc)
                .Sum(o => o.TotalPrice);

            points.Add(new RevenuePointDto
            {
                Date = dayStartUtc.ToEgyptTime(),
                Revenue = dayRevenue
            });
        }

        return points;
    }



    private List<CategoryShareDto> BuildCategoryShare(List<Domain.Entities.Order> orders)
    {
        var allItems = orders.SelectMany(o => o.OrderItems).ToList();
        var totalCount = allItems.Count;

        if (totalCount == 0) return new List<CategoryShareDto>();

        return allItems
            .GroupBy(oi => oi.Product?.Category?.Name ?? "Other")
            .Select(g => new CategoryShareDto
            {
                CategoryName = g.Key,
                OrdersCount = g.Count(),
                Percentage = Math.Round((decimal)g.Count() / totalCount * 100, 1)
            })
            .OrderByDescending(c => c.Percentage)
            .ToList();
    }

    private async Task<TodayOverviewDto> BuildTodayOverviewAsync(DateTime todayStartUtc, DateTime todayEndUtc)
    {
        var yesterdayStartUtc = todayStartUtc.AddDays(-1);
        var yesterdayEndUtc = todayStartUtc;

        var todayOrders = await _orderRepository.GetOrdersInRangeAsync(todayStartUtc, todayEndUtc);
        var yesterdayOrders = await _orderRepository.GetOrdersInRangeAsync(yesterdayStartUtc, yesterdayEndUtc);

        var todayRevenue = todayOrders.Sum(o => o.TotalPrice);
        var yesterdayRevenue = yesterdayOrders.Sum(o => o.TotalPrice);

        var itemsSold = todayOrders.SelectMany(o => o.OrderItems).Sum(oi => oi.Quantity);

        var branches = (await _branchRepository.GetAllAsync()).ToList();
        var openBranchesCount = branches.Count(b => b.IsActive);

        var stockSummary = await _inventoryService.GetStockSummaryAsync();

        return new TodayOverviewDto
        {
            OpenBranches = $"{openBranchesCount}/{branches.Count}",
            ItemsSold = itemsSold,
            StockAlertsCount = stockSummary.LowStockCount + stockSummary.OutOfStockCount,
            TodayRevenue = todayRevenue,
            TodayRevenueChangePercentage = CalculateChange(todayRevenue, yesterdayRevenue)
        };
    }

    private async Task<DashboardAlertsDto> BuildAlertsAsync()
    {
        var stockSummary = await _inventoryService.GetStockSummaryAsync();
        var needsAttention = await _branchService.GetNeedsAttentionAsync();
        var onLeaveCount = await _employeeRepository.GetOnLeaveCountAsync();

        var needsAttentionCount = needsAttention.Count();

        return new DashboardAlertsDto
        {
            LowStockCount = stockSummary.LowStockCount,
            OutOfStockCount = stockSummary.OutOfStockCount,
            BranchesNeedingAttentionCount = needsAttentionCount,
            EmployeesOnLeaveCount = onLeaveCount,
            TotalAlertsCount = stockSummary.LowStockCount + stockSummary.OutOfStockCount
                + needsAttentionCount + onLeaveCount
        };
    }

    private static decimal? CalculateChange(decimal current, decimal previous)
    {
        if (previous == 0)
            return current == 0 ? 0 : null; 
        return Math.Round((current - previous) / previous * 100, 1);
    }

    private static decimal? CalculateChange(int current, int previous)
        => CalculateChange((decimal)current, (decimal)previous);
}