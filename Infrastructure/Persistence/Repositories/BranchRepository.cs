using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class BranchRepository : IBranchRepository
{
    private readonly AppDbContext _context;

    public BranchRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Branch?> GetByIdAsync(int id)
        => await _context.Branches.FindAsync(id);

    public async Task<IEnumerable<Branch>> GetAllAsync()
        => await _context.Branches.Where(b=>b.IsActive).ToListAsync();

    public async Task AddAsync(Branch branch)
    {
        await _context.Branches.AddAsync(branch);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await _context.Branches.CountAsync();
    }



    public async Task UpdateAsync(Branch branch)
    {
        _context.Branches.Update(branch);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
        => await _context.Branches.AnyAsync(b => b.Id == id);

    public async Task<IEnumerable<Order>> GetOrdersByBranchAsync(int branchId)
        => await _context.Orders
            .Where(o => o.BranchId == branchId)
            .ToListAsync();

    public async Task<IEnumerable<Branch>> SearchAsync(string keyword)
    {
        var keywords = keyword.Split(new[] { '+', ' ' },
            StringSplitOptions.RemoveEmptyEntries);

        return await _context.Branches
            .Where(b => keywords.Any(k =>
                b.Name.Contains(k) ||
                b.City.Contains(k) ||
                b.Address.Contains(k)))
            .ToListAsync();
    }

    public async Task<IEnumerable<Branch>> FuzzySearchAsync(string keyword)
    {
        var allBranches = await _context.Branches.ToListAsync();

        var searchWords = keyword.Split(new[] { '+', ' ' },
            StringSplitOptions.RemoveEmptyEntries);

        return allBranches
            .Where(b => searchWords.Any(sw =>
                b.Name.Split(' ').Any(pw => LevenshteinDistance(pw, sw) <= 2) ||
                b.City.Split(' ').Any(pw => LevenshteinDistance(pw, sw) <= 2)))
            .ToList();
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


    public async Task<int> GetActiveBranchesCountAsync()
    {
        return await _context.Branches.CountAsync(b => b.IsActive);
    }
}