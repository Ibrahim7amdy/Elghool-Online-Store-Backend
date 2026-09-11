using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly AppDbContext _context;

    public AdminRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Admin?> GetByEmailAsync(string email)
        => await _context.Admins.FirstOrDefaultAsync(a => a.Email == email);

    public async Task<Admin?> GetByIdAsync(int id)
        => await _context.Admins.FindAsync(id);

    public async Task AddAsync(Admin admin)
    {
        await _context.Admins.AddAsync(admin);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> EmailExistsAsync(string email)
        => await _context.Admins.AnyAsync(a => a.Email == email);

    public async Task UpdateAsync(Admin admin)
    {
        _context.Admins.Update(admin);
        await _context.SaveChangesAsync();
    }
}