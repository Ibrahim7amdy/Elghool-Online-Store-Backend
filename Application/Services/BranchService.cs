using Application.DTOs.Branch;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class BranchService
{
    private readonly IBranchRepository _branchRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public BranchService(IBranchRepository branchRepository, IEmployeeRepository employeeRepository)
    {
        _branchRepository = branchRepository;
        _employeeRepository = employeeRepository;
    }

    public async Task<IEnumerable<BranchDto>> GetAllAsync()
    {
        var branches = await _branchRepository.GetAllAsync();
        return branches.Select(MapToDto);
    }

    public async Task<BranchDto?> GetByIdAsync(int id)
    {
        var branch = await _branchRepository.GetByIdAsync(id);
        return branch == null ? null : MapToDto(branch);
    }

    public async Task<BranchDto> CreateAsync(BranchInputDto dto)
    {
        var branch = new Branch
        {
            Name = dto.Name,
            Address = dto.Address,
            City = dto.City,
            PhoneNumber = dto.PhoneNumber,
            OpeningTime = dto.OpeningTime,
            ClosingTime = dto.ClosingTime,
            IsActive = dto.IsActive ?? true,
            RevenueTarget = dto.RevenueTarget
        };

        await _branchRepository.AddAsync(branch);
        return MapToDto(branch);
    }

    public async Task<BranchDto?> UpdateAsync(int id, BranchInputDto dto)
    {
        var branch = await _branchRepository.GetByIdAsync(id);
        if (branch == null) return null;

        branch.Name = dto.Name;
        branch.Address = dto.Address;
        branch.City = dto.City;
        branch.PhoneNumber = dto.PhoneNumber;
        branch.OpeningTime = dto.OpeningTime;
        branch.ClosingTime = dto.ClosingTime;
        if (dto.IsActive.HasValue)
            branch.IsActive = dto.IsActive.Value;
        branch.RevenueTarget = dto.RevenueTarget;

        await _branchRepository.UpdateAsync(branch);
        return MapToDto(branch);
    }

    public async Task<BranchStatsDto?> GetBranchStatsAsync(int branchId)
    {
        var branch = await _branchRepository.GetByIdAsync(branchId);
        if (branch == null) return null;

        var now = DateTime.UtcNow;
        var currentMonthStart = new DateTime(now.Year, now.Month, 1);
        var lastMonthStart = currentMonthStart.AddMonths(-1);

        var orders = await _branchRepository.GetOrdersByBranchAsync(branchId);

        var currentRevenue = orders
            .Where(o => o.CreatedAt >= currentMonthStart)
            .Sum(o => o.TotalPrice);

        var lastRevenue = orders
            .Where(o => o.CreatedAt >= lastMonthStart && o.CreatedAt < currentMonthStart)
            .Sum(o => o.TotalPrice);

        var revenueChange = lastRevenue == 0
            ? (decimal?)null
            : Math.Round((currentRevenue - lastRevenue) / lastRevenue * 100, 1);

        var Now = TimeOnly.FromDateTime(DateTime.Now);

        return new BranchStatsDto
        {
            Id = branch.Id,
            Name = branch.Name,
            Address = branch.Address,
            City = branch.City,
            IsActive = branch.IsActive,
            IsOpenNow = branch.IsActive ? Now >= branch.OpeningTime && Now <= branch.ClosingTime : false,
            CurrentMonthRevenue = currentRevenue,
            LastMonthRevenue = lastRevenue,
            RevenueChangePercentage = revenueChange,
            OrdersCount = orders.Count(),
            CustomersCount = orders.Select(o => o.CustomerId).Distinct().Count(),
            RevenueTarget = branch.RevenueTarget,
            TargetAchievementPercentage = branch.RevenueTarget.HasValue && branch.RevenueTarget > 0
                ? Math.Round(currentRevenue / branch.RevenueTarget.Value * 100, 1)
                : null
        };
    }

    //public async Task<IEnumerable<BranchStatsDto>> GetAllBranchesStatsAsync()
    //{
    //    var branches = await _branchRepository.GetAllAsync();
    //    var stats = new List<BranchStatsDto>();

    //    foreach (var branch in branches)
    //    {
    //        var stat = await GetBranchStatsAsync(branch.Id);
    //        if (stat != null) stats.Add(stat);
    //    }

    //    return stats.OrderByDescending(s => s.CurrentMonthRevenue).ToList();
    //}

    public async Task<IEnumerable<BranchStatsDto>> GetTopPerformersAsync()
    {
        var allStats = await GetFilteredBranchesStatsAsync(null);
        return allStats.Take(11).ToList();
    }

    public async Task<IEnumerable<BranchStatsDto>> GetNeedsAttentionAsync()
    {
        var allStats = await GetFilteredBranchesStatsAsync(null);
        return allStats
            .Where(s => s.RevenueChangePercentage < 0)
            .OrderBy(s => s.RevenueChangePercentage)
            .ToList();
    }




    public async Task<bool> ToggleStatusAsync(int id)
    {
        var branch = await _branchRepository.GetByIdAsync(id);
        if (branch == null) return false;

        branch.IsActive = !branch.IsActive;
        await _branchRepository.UpdateAsync(branch);
        return true;
    }

    public async Task<IEnumerable<BranchStatsDto>> GetFilteredBranchesStatsAsync(BranchFilterDto filter)
    {
        filter ??= new BranchFilterDto();

        IEnumerable<Branch> branches;

        // 1. فلتر البحث (Search)
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            branches = await _branchRepository.SearchAsync(filter.Search);
            if (!branches.Any())
                branches = await _branchRepository.FuzzySearchAsync(filter.Search);
        }
        else
        {
            branches = await _branchRepository.GetAllAsync();
        }

        // 2. فلتر الحالة (Status)
        if (filter.IsOpen.HasValue)
        {
            var now = TimeOnly.FromDateTime(DateTime.Now);

            if (filter.IsOpen.Value) branches = branches.Where(b => b.IsActive && now >= b.OpeningTime && now <= b.ClosingTime);
            else branches = branches.Where(b => !b.IsActive || now < b.OpeningTime || now > b.ClosingTime);

        }

        // 3. تجميع الإحصائيات
        var stats = new List<BranchStatsDto>();
        foreach (var branch in branches)
        {
            var stat = await GetBranchStatsAsync(branch.Id);
            if (stat != null) stats.Add(stat);
        }

        // 4. تطبيق الترتيب (Sorting)
        IEnumerable<BranchStatsDto> resultQuery = stats;
        bool isDesc = string.IsNullOrWhiteSpace(filter.SortDir) || filter.SortDir.ToLower() == "desc";

        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            resultQuery = filter.SortBy.ToLower() switch
            {
                "orders" => isDesc ? resultQuery.OrderByDescending(x => x.OrdersCount) : resultQuery.OrderBy(x => x.OrdersCount),
                "revenue" => isDesc ? resultQuery.OrderByDescending(x => x.CurrentMonthRevenue) : resultQuery.OrderBy(x => x.CurrentMonthRevenue),
                _ => resultQuery.OrderByDescending(x => x.CurrentMonthRevenue)
            };
        }
        else
        {
            resultQuery = resultQuery.OrderByDescending(x => x.CurrentMonthRevenue);
        }

        return resultQuery.ToList();
    }

    private static BranchDto MapToDto(Branch branch) => new()
    {
        Id = branch.Id,
        Name = branch.Name,
        Address = branch.Address,
        City = branch.City,
        PhoneNumber = branch.PhoneNumber,
        OpeningTime = branch.OpeningTime,
        ClosingTime = branch.ClosingTime,
        IsActive = branch.IsActive,
        RevenueTarget = branch.RevenueTarget

    };
}