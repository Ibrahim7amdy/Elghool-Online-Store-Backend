using System;
using System.Collections.Generic;
using System.Text;
using Application.DTOs.Branch;
using FluentValidation;

namespace Application.Validators.Branch;

public class BranchInputValidator : AbstractValidator<BranchInputDto>
{
    public BranchInputValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("اسم الفرع مطلوب.")
            .MaximumLength(100).WithMessage("اسم الفرع لا يتخطى 100 حرف.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("العنوان مطلوب.")
            .MaximumLength(300).WithMessage("العنوان لا يتخطى 300 حرف.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("المدينة مطلوبة.")
            .MaximumLength(100).WithMessage("المدينة لا تتخطى 100 حرف.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("رقم الهاتف مطلوب.")
            .Matches(@"^01[0125][0-9]{8}$").WithMessage("رقم الهاتف غير صحيح.");

        RuleFor(x => x.OpeningTime)
            .NotEmpty().WithMessage("وقت الفتح مطلوب");

        RuleFor(x => x.ClosingTime)
            .NotEmpty().WithMessage("وقت الغلق مطلوب")
            .Must((dto, closingTime) => closingTime > dto.OpeningTime)
            .WithMessage("وقت الغلق لازم يكون بعد وقت الفتح");

        RuleFor(x => x.RevenueTarget)
            .GreaterThan(0).WithMessage("الـ Target لازم يكون أكبر من 0.")
            .When(x => x.RevenueTarget.HasValue);
    }
}