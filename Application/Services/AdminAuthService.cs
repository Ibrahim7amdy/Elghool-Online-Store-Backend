using Application.DTOs.Admin;
using Application.DTOs.Auth;
using Application.Interfaces.Repositories;
using Application.Interfaces.Security;
using Domain.Entities;

namespace Application.Services;

public class AdminAuthService
{
    private readonly IAdminRepository _adminRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AdminAuthService(
        IAdminRepository adminRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _adminRepository = adminRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AdminAuthResponseDto> LoginAsync(AdminLoginDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

        var admin = await _adminRepository.GetByEmailAsync(normalizedEmail);

        if (admin == null || !_passwordHasher.Verify(dto.Password, admin.PasswordHash))
            throw new Exception("الإيميل أو الباسورد غلط");

        return new AdminAuthResponseDto
        {
            Token = _tokenService.GenerateAdminToken(admin),
            Email = admin.Email,
            FirstName = admin.FirstName,
            LastName = admin.LastName,
        };
    }

    public async Task<string> CreateAdminAsync(CreateAdminDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

        if (await _adminRepository.EmailExistsAsync(normalizedEmail))
            throw new Exception("هذا الإيميل مستخدم لأدمن آخر مسبقاً.");

        var newAdmin = new Admin
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = normalizedEmail,
            PasswordHash = _passwordHasher.Hash(dto.Password),
            IsSuperAdmin = false
        };


        await _adminRepository.AddAsync(newAdmin);

        return "تم إضافة الأدمن بنجاح.";
    }
}