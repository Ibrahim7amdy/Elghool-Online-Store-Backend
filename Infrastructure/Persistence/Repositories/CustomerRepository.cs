using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByEmailAsync(string email)
        => await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);

    public async Task<Customer?> GetByPhoneNumberAsync(string phoneNumber)
        => await _context.Customers.FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);

    public async Task<Customer?> GetByIdAsync(int id)
        => await _context.Customers.FindAsync(id);

    public async Task AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> EmailExistsAsync(string email)
        => await _context.Customers.AnyAsync(c => c.Email == email);

    public async Task<bool> PhoneNumberExistsAsync(string phoneNumber)
        => await _context.Customers.AnyAsync(c => c.PhoneNumber == phoneNumber);

    public async Task<bool> PhoneNumberExistsForOtherCustomerAsync(int customerId, string phoneNumber)
    => await _context.Customers.AnyAsync(c =>
        c.PhoneNumber == phoneNumber &&
        c.Customer_Id != customerId);

    public async Task UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }
    public async Task<Customer?> GetByGoogleIdAsync(string googleId)
    => await _context.Customers.FirstOrDefaultAsync(c => c.GoogleId == googleId);



    public async Task<IEnumerable<Customer>> GetAllAsync()
    => await _context.Customers
        .Include(c => c.PreferredBranch)
        .Include(c => c.Orders)
        .ToListAsync();

    public async Task<IEnumerable<Customer>> GetByBranchAsync(int branchId)
        => await _context.Customers
            .Include(c => c.PreferredBranch)
            .Include(c => c.Orders)
            .Where(c => c.PreferredBranchId == branchId)
            .ToListAsync();

    public async Task<IEnumerable<Customer>> GetNewThisMonthAsync()
    {
        var firstDayOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        return await _context.Customers
            .Include(c => c.PreferredBranch)
            .Include(c => c.Orders)
            .Where(c => c.CreatedAt >= firstDayOfMonth)
            .ToListAsync();
    }

    public async Task<IEnumerable<Customer>> GetTopSpendersAsync(int count = 10)
        => await _context.Customers
            .Include(c => c.PreferredBranch)
            .Include(c => c.Orders)
            .OrderByDescending(c => c.Orders
                .Sum(o => o.TotalPrice))
            .Take(count)
            .ToListAsync();

    public async Task<IEnumerable<Customer>> SearchAsync(string keyword)
    {
        var keywords = keyword.Split(new[] { '+', ' ' },
            StringSplitOptions.RemoveEmptyEntries);

        return await _context.Customers
            .Include(c => c.PreferredBranch)
            .Include(c => c.Orders)
            .Where(c => keywords.Any(k =>
                c.FirstName.Contains(k) ||
                c.LastName.Contains(k) ||
                c.Email.Contains(k) ||
                (c.PhoneNumber != null && c.PhoneNumber.Contains(k))))
            .ToListAsync();
    }

    public async Task<IEnumerable<Customer>> FuzzySearchAsync(string keyword)
    {
        var allCustomers = await _context.Customers
            .Include(c => c.PreferredBranch)
            .Include(c => c.Orders)
            .ToListAsync();

        var searchWords = keyword.Split(new[] { '+', ' ' },
            StringSplitOptions.RemoveEmptyEntries);

        return allCustomers
            .Where(c => searchWords.Any(sw =>
                c.FirstName.Split(' ').Any(pw => LevenshteinDistance(pw, sw) <= 3) ||
                c.LastName.Split(' ').Any(pw => LevenshteinDistance(pw, sw) <= 3)))
            .ToList();
    }

    public async Task ToggleStatusAsync(int customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer == null) return;
        customer.IsActive = !customer.IsActive;
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
}