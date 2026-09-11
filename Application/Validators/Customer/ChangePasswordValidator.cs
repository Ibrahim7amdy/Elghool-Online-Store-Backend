using Application.DTOs.Customer;
using FluentValidation;

namespace Application.Validators.Customer;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("الباسورد الحالي مطلوب.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("الباسورد الجديد مطلوب.")
            .MinimumLength(8).WithMessage("الباسورد لازم يكون 8 أحرف على الأقل.")
            .Matches(@"[A-Z]").WithMessage("لازم يحتوي على حرف كبير.")
            .Matches(@"[a-z]").WithMessage("لازم يحتوي على حرف صغير.")
            .Matches(@"[0-9]").WithMessage("لازم يحتوي على رقم.")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("لازم يحتوي على رمز.");
    }
}