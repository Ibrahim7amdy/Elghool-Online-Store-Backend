using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators.Auth;

public class CustomerLoginValidator : AbstractValidator<CustomerLoginDto>
{
    public CustomerLoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("الإيميل مطلوب")
            .EmailAddress().WithMessage("صيغة الإيميل غير صحيحة");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("الباسورد مطلوب");
    }
}