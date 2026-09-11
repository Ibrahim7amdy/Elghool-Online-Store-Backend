using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class PasswordResetCodeRepository : IPasswordResetCodeRepository
{
    private readonly AppDbContext _context;

    public PasswordResetCodeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PasswordResetCode code)
    {
        await _context.PasswordResetCodes.AddAsync(code);
        await _context.SaveChangesAsync();
    }

    public async Task<PasswordResetCode?> GetValidCodeAsync(string email, string code)
        => await _context.PasswordResetCodes
            .FirstOrDefaultAsync(x =>
                x.Email == email &&
                x.Code == code &&
                !x.IsUsed &&
                x.ExpiresAt > DateTime.UtcNow);

    public async Task DeleteAllByEmailAsync(string email)
    {
        var codes = await _context.PasswordResetCodes
            .Where(x => x.Email == email)
            .ToListAsync();

        _context.PasswordResetCodes.RemoveRange(codes);
        await _context.SaveChangesAsync();
    }
}