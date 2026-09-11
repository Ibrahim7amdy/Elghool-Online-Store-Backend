using Application.DTOs.Employee;
using FluentValidation;

namespace Application.Validators.Employee;

public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeDto>
{
    public CreateEmployeeValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("الاسم مطلوب.")
            .MaximumLength(100).WithMessage("الاسم لا يتخطى 100 حرف.");

        RuleFor(x => x.BranchId)
            .GreaterThan(0).WithMessage("الفرع مطلوب.");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("الدور غير صحيح.");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^01[0125][0-9]{8}$").WithMessage("رقم الموبايل غير صحيح.")
            .When(x => x.PhoneNumber != null);

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("الإيميل غير صحيح.")
            .When(x => x.Email != null);

        RuleFor(x => x.WorkStart)
            .LessThan(x => x.WorkEnd).WithMessage("وقت البداية لازم يكون قبل وقت النهاية.");
    }

}