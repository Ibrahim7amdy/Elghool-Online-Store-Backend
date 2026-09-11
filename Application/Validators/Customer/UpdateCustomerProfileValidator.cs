using Application.DTOs.Customer;
using FluentValidation;

public class UpdateCustomerProfileValidator : AbstractValidator<UpdateCustomerProfileDto>
{
    public UpdateCustomerProfileValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("الاسم الأول مطلوب.")
            .MaximumLength(50).WithMessage("الاسم الأول لا يتخطى 50 حرف.");

        RuleFor(x => x.LastName)
            .MaximumLength(50).WithMessage("الاسم الأخير لا يتخطى 50 حرف.")
            .When(x => !string.IsNullOrEmpty(x.LastName));

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("رقم الموبايل مطلوب.")
            .Matches(@"^01[0125][0-9]{8}$").WithMessage("رقم الموبايل المصري غير صحيح.");

        RuleFor(x => x.PreferredBranchId)
            .GreaterThan(0).WithMessage("رقم الفرع غير صحيح.")
            .When(x => x.PreferredBranchId.HasValue);
    }
}