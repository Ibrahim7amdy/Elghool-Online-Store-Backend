using Application.DTOs.Cart;
using FluentValidation;

public class UpdateCartItemValidator : AbstractValidator<UpdateCartItemDto>
{
    public UpdateCartItemValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("الكمية لازم تكون أكبر من 0.");
    }
}