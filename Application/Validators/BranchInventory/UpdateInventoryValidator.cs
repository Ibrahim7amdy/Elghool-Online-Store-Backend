using Application.DTOs.BranchInventory;
using FluentValidation;

namespace Application.Validators.BranchInventory

{
    public class UpdateInventoryValidator : AbstractValidator<AddInventoryDto>
    {
        public UpdateInventoryValidator()
        {

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("الكمية لازم تكون 0 أو أكبر.");

            RuleFor(x => x.LowStockThreshold)
            .GreaterThanOrEqualTo(0)
            .When(x => x.LowStockThreshold.HasValue)
            .WithMessage("حد التنبيه للمخزون يجب أن يكون 0 أو أكثر.");


            RuleFor(x => x.Notes)
                .MaximumLength(500)
                .WithMessage("الملاحظات لا يجب أن تتجاوز 500 حرف.");


        }
    }
}
