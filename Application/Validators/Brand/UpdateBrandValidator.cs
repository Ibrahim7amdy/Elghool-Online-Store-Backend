using Application.DTOs.Brand;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validators.Brand
{
    public class UpdateBrandValidator : AbstractValidator<UpdateBrandDto>
    {
        public UpdateBrandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("اسم البراند مطلوب.")
                .MaximumLength(100).WithMessage("اسم البراند لا يتخطى 100 حرف.");
        }

    }
}
