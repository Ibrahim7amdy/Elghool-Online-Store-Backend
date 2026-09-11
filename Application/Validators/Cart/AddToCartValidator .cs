using Application.DTOs.Cart;
using FluentValidation;

namespace Application.Validators.Cart;

public class AddToCartValidator : AbstractValidator<AddToCartDto>
{
    public AddToCartValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("المنتج مطلوب.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("الكمية لازم تكون أكبر من 0.");
    }
}