using Application.DTOs.Order;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validators.Order
{
    public class UpdateOrderStatusValidator : AbstractValidator<UpdateOrderStatusDto>
    {
        public UpdateOrderStatusValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("حالة الأوردر غير صحيحة.");
        }
    }
}
