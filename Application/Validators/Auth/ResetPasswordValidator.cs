using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators.Auth;

public class ResetPasswordValidator : AbstractValidator<CompleteResetPasswordDto>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("الإيميل مطلوب")
            .EmailAddress().WithMessage("صيغة الإيميل غير صحيحة");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("الباسورد الجديد مطلوب")
            .MinimumLength(8).WithMessage("الباسورد لازم يكون 8 حروف على الأقل")
            .Matches("[A-Z]").WithMessage("لازم يحتوي على حرف كبير")
            .Matches("[0-9]").WithMessage("لازم يحتوي على رقم");
    }
}