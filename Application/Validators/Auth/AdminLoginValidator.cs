using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators.Auth;

public class AdminLoginValidator : AbstractValidator<AdminLoginDto>
{
    public AdminLoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("الإيميل مطلوب")
            .EmailAddress().WithMessage("صيغة الإيميل غير صحيحة");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("الباسورد مطلوب");
    }
}