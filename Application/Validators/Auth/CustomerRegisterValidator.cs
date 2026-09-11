using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators.Auth;

public class CustomerRegisterValidator : AbstractValidator<CustomerRegisterDto>
{
    public CustomerRegisterValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("الاسم الأول مطلوب")
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("الإيميل مطلوب")
            .EmailAddress().WithMessage("صيغة الإيميل غير صحيحة");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("رقم الموبايل مطلوب")
            .Matches(@"^01[0125][0-9]{8}$").WithMessage("رقم الموبايل غير صحيح");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("الباسورد مطلوب")
            .MinimumLength(8).WithMessage("الباسورد لازم يكون 8 حروف على الأقل")
            .Matches("[A-Z]").WithMessage("لازم يحتوي على حرف كبير")
            .Matches("[0-9]").WithMessage("لازم يحتوي على رقم");
      
    }
}