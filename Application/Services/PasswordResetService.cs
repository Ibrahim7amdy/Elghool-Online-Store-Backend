using Application.DTOs.Auth;
using Application.Interfaces.Repositories;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Application.Services;

public class PasswordResetService
{
    private readonly IPasswordResetCodeRepository _resetCodeRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IAdminRepository _adminRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;

    public PasswordResetService(
        IPasswordResetCodeRepository resetCodeRepository,
        ICustomerRepository customerRepository,
        IAdminRepository adminRepository,
        IPasswordHasher passwordHasher,
        IEmailService emailService)
    {
        _resetCodeRepository = resetCodeRepository;
        _customerRepository = customerRepository;
        _adminRepository = adminRepository;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
    }

    // الخطوة 1: استقبال الإيميل وتوليد الكود وإرساله
    public async Task SendResetCodeAsync(ForgotPasswordDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

        var customerExists = await _customerRepository.EmailExistsAsync(normalizedEmail);
        var adminExists = await _adminRepository.EmailExistsAsync(normalizedEmail);

        if (!customerExists && !adminExists)
            throw new Exception("الإيميل ده مش مسجل في النظام");

        await _resetCodeRepository.DeleteAllByEmailAsync(normalizedEmail);

        var code = new Random().Next(100000, 999999).ToString();

        var resetCode = new PasswordResetCode
        {
            Email = normalizedEmail,
            Code = code,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false
        };

        await _resetCodeRepository.AddAsync(resetCode);
        await _emailService.SendPasswordResetCodeAsync(normalizedEmail, code);
    }

    // الخطوة 2: التأكد من صحة الكود لإعطاء التمام للفرونت
    public async Task<bool> VerifyResetCodeAsync(VerifyResetCodeDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

        var resetCode = await _resetCodeRepository.GetValidCodeAsync(normalizedEmail, dto.Code);

        if (resetCode == null)
            throw new Exception("الكود غلط أو انتهت صلاحيته");

        return true;
    }

    // الخطوة 3: استقبال الباسورد الجديد وحفظه (مع تمرير الكود للأمان)
    public async Task CompleteResetPasswordAsync(CompleteResetPasswordDto dto, string code)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

        // تأكيد أخير للأمان: نتأكد إن الكود لسه صالح ومطابق قبل ما نغير الباسورد
        var resetCode = await _resetCodeRepository.GetValidCodeAsync(normalizedEmail, code);
        if (resetCode == null)
            throw new Exception("عملية غير مصرح بها أو انتهت صلاحية الكود");

        var newHashedPassword = _passwordHasher.Hash(dto.NewPassword);

        var customer = await _customerRepository.GetByEmailAsync(normalizedEmail);
        if (customer != null)
        {
            customer.PasswordHash = newHashedPassword;
            await _customerRepository.UpdateAsync(customer);
        }
        else
        {
            var admin = await _adminRepository.GetByEmailAsync(normalizedEmail);
            if (admin != null)
            {
                admin.PasswordHash = newHashedPassword;
                await _adminRepository.UpdateAsync(admin);
            }
        }

        // مسح الكود فوراً بعد نجاح العملية عشان محدش يستخدمه تاني
        await _resetCodeRepository.DeleteAllByEmailAsync(normalizedEmail);
    }
}