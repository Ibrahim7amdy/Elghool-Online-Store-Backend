using Application.DTOs.Customer;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Helpers;

namespace Application.Services;

public class AdminCustomerService : IAdminCustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IBranchRepository _branchRepository;

    public AdminCustomerService(
        ICustomerRepository customerRepository,
        IBranchRepository branchRepository)
    {
        _customerRepository = customerRepository;
        _branchRepository = branchRepository;
    }

    public async Task<IEnumerable<AdminCustomerDto>> GetAllAsync()
    {
        var customers = await _customerRepository.GetAllAsync();
        return customers.Select(MapToDto).ToList();
    }

    public async Task<AdminCustomerDto?> GetByIdAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        return customer == null ? null : MapToDto(customer);
    }

    public async Task<IEnumerable<AdminCustomerDto>> GetByBranchAsync(int branchId)
    {
        var customers = await _customerRepository.GetByBranchAsync(branchId);
        return customers.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<AdminCustomerDto>> GetTopSpendersAsync()
    {
        var customers = await _customerRepository.GetTopSpendersAsync();
        return customers.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<AdminCustomerDto>> GetNewThisMonthAsync()
    {
        var customers = await _customerRepository.GetNewThisMonthAsync();
        return customers.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<AdminCustomerDto>> SearchAsync(string keyword)
    {
        var customers = await _customerRepository.SearchAsync(keyword);

        if (!customers.Any())
            customers = await _customerRepository.FuzzySearchAsync(keyword);

        return customers.Select(MapToDto).ToList();
    }

    public async Task<CustomerStatsDto> GetStatsAsync()
    {
        var all = await _customerRepository.GetAllAsync();
        var allList = all.ToList();

        var now = DateTime.UtcNow.ToEgyptTime();
        var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
        var lastMonth = firstDayOfMonth.AddMonths(-1);

        var newThisMonth = allList.Count(c => c.CreatedAt.ToEgyptTime() >= firstDayOfMonth);
        var newLastMonth = allList.Count(c => {
            var created = c.CreatedAt.ToEgyptTime();
            return created >= lastMonth && created < firstDayOfMonth;
        });

        var newMonthPercentage = newLastMonth == 0 ? 0
            : Math.Round((decimal)(newThisMonth - newLastMonth) / newLastMonth * 100, 1);

        var totalSpent = allList.Sum(c => c.Orders
            .Sum(o => o.TotalPrice));

        var avgSpend = allList.Count == 0 ? 0 : Math.Round(totalSpent / allList.Count, 2);


        var activeToday = allList.Count(c => c.Orders
            .Any(o => o.CreatedAt.ToEgyptTime().Date == now.Date));


        var monthlyGrowth = Enumerable.Range(0, 6)
            .Select(i => 5 - i)
            .Select(offset => {
                var month = now.AddMonths(-offset);
                var start = new DateTime(month.Year, month.Month, 1);
                var end = start.AddMonths(1);
                return new MonthlyGrowthDto
                {
                    Month = month.ToString("MMM"),
                    Count = allList.Count(c => {
                        var created = c.CreatedAt.ToEgyptTime();
                        return created >= start && created < end;
                    })
                };
            })
            .ToList();

        // Tier Stats
        var tierStats = new CustomerTierStatsDto
        {
            VipCount = allList.Count(c => c.Orders
                .Sum(o => o.TotalPrice) >= 500),
            RegularCount = allList.Count(c => {
                var spent = c.Orders
                    .Sum(o => o.TotalPrice);
                return spent >= 100 && spent < 500;
            }),
            OccasionalCount = allList.Count(c => c.Orders
                .Sum(o => o.TotalPrice) < 100)
        };

        return new CustomerStatsDto
        {
            TotalCustomers = allList.Count,
            NewThisMonth = newThisMonth,
            NewThisMonthPercentage = newMonthPercentage,
            AvgSpendPerCustomer = avgSpend,
            ActiveToday = activeToday,
            MonthlyGrowth = monthlyGrowth,
            TierStats = tierStats
        };
    }


    public async Task<bool> ToggleStatusAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null) return false;

        await _customerRepository.ToggleStatusAsync(id);
        return true;
    }

    private static AdminCustomerDto MapToDto(Customer c)
    {
        var validOrders = c.Orders?
            .Where(o => o.Status == OrderStatus.PickedUp)
            .ToList() ?? new();

        var totalSpent = validOrders.Sum(o => o.TotalPrice);

        return new AdminCustomerDto
        {
            Id = c.Customer_Id,
            FirstName = c.FirstName,

            LastName = string.IsNullOrWhiteSpace(c.LastName) ? null : c.LastName.Trim(),

            Email = c.Email,
            PreferredBranchName = c.PreferredBranch?.Name,
            TotalOrders = validOrders.Count,
            TotalSpent = totalSpent,
            IsActive = c.IsActive,
            LastVisit = c.Orders?.MaxBy(o => o.CreatedAt)?.CreatedAt.ToEgyptTime(),
            CreatedAt = c.CreatedAt.ToEgyptTime(),

            CustomerTier = totalSpent >= 500 ? "VIP" :
                           totalSpent >= 100 ? "Regular" : "Occasional"
        };
    }
}