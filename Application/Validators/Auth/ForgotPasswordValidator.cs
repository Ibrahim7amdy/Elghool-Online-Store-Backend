using System;
using System.Collections.Generic;
using System.Text;
using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators.Auth;

public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordDto>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("الإيميل مطلوب")
            .EmailAddress().WithMessage("صيغة الإيميل غير صحيحة");
    }
}