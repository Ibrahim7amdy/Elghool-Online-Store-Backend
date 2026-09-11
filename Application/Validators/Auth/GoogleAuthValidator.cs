using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators.Auth;

public class GoogleAuthValidator : AbstractValidator<GoogleAuthDto>
{
    public GoogleAuthValidator()
    {
        RuleFor(x => x.IdToken)
            .NotEmpty().WithMessage("التوكن مطلوب");

        // بس لو بيبعت البيانات الإضافية
        When(x => x.PhoneNumber != null, () =>
        {
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^01[0125][0-9]{8}$").WithMessage("رقم الموبايل غير صحيح");
        });

        When(x => x.PreferredBranchId != null, () =>
        {
            RuleFor(x => x.PreferredBranchId)
                .GreaterThan(0).WithMessage("الفرع غير صحيح");
        });
    }
}