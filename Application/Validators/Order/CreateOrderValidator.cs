using Application.DTOs.Order;
using FluentValidation;

namespace Application.Validators.Order;

public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.BranchId)
            .GreaterThan(0).WithMessage("الفرع غير صحيح.")
            .When(x => x.BranchId.HasValue); 

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("لازم تضيف منتج واحد على الأقل.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("المنتج مطلوب.");

            item.RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("الكمية لازم تكون أكبر من 0.");
        });
    }
}

