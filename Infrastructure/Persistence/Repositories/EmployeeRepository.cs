using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;



namespace Infrastructure.Persistence.Repositories;
public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context)
    {
        _context = context;
    }

    private IQueryable<Employee> GetWithIncludes()
        => _context.Employees
            .Include(e => e.Branch);

    public async Task<Employee?> GetByIdAsync(int id)
        => await GetWithIncludes().FirstOrDefaultAsync(e => e.Id == id);

    public async Task<IEnumerable<Employee>> GetAllAsync()
        => await GetWithIncludes().ToListAsync();

    public async Task<IEnumerable<Employee>> GetByBranchAsync(int branchId)
        => await GetWithIncludes()
            .Where(e => e.BranchId == branchId)
            .ToListAsync();

    public async Task<IEnumerable<Employee>> GetByRoleAsync(EmployeeRole role)
        => await GetWithIncludes()
            .Where(e => e.Role == role)
            .ToListAsync();

    public async Task<IEnumerable<Employee>> GetOnShiftNowAsync()
    {
        var now = TimeOnly.FromDateTime(DateTime.Now);
        return await GetWithIncludes()
            .Where(e => e.IsActive == true)
            .Where(e => e.WorkStart <= now && e.WorkEnd >= now)
            .ToListAsync();
    }

    public async Task<IEnumerable<Employee>> SearchAsync(string keyword)
    {
        var keywords = keyword.Split(new[] { '+', ' ' },
            StringSplitOptions.RemoveEmptyEntries);

        return await GetWithIncludes()
            .Where(e => keywords.Any(k =>
                e.FullName.Contains(k) ||
                e.EmployeeCode.Contains(k)))
            .ToListAsync();
    }

    public async Task<IEnumerable<Employee>> FuzzySearchAsync(string keyword)
    {
        var allEmployees = await GetWithIncludes().ToListAsync();

        var searchWords = keyword.Split(new[] { '+', ' ' },
            StringSplitOptions.RemoveEmptyEntries);

        return allEmployees
            .Where(e => searchWords.Any(sw =>
                e.FullName.Split(' ').Any(pw => LevenshteinDistance(pw, sw) <= 2)))
            .ToList();
    }

    public async Task<string> GenerateEmployeeCodeAsync()
    {
        var count = await _context.Employees.CountAsync();
        return $"E{(count + 1):D4}";
    }

    public async Task AddAsync(Employee employee)
    {
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Employee employee)
    {
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();
    }

    private int LevenshteinDistance(string s, string t)
    {
        int n = s.Length, m = t.Length;
        var d = new int[n + 1, m + 1];

        for (int i = 0; i <= n; i++) d[i, 0] = i;
        for (int j = 0; j <= m; j++) d[0, j] = j;

        for (int i = 1; i <= n; i++)
            for (int j = 1; j <= m; j++)
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + (s[i - 1] == t[j - 1] ? 0 : 1));

        return d[n, m];
    }



    public async Task<int> GetOnLeaveCountAsync()
    => await _context.Employees
        .CountAsync(e => e.IsActive && e.Status == EmployeeStatus.OnLeave);
}