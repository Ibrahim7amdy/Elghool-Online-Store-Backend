// Application/Validators/Product/CreateProductValidator.cs
using Application.DTOs.Product;
using FluentValidation;

namespace Application.Validators.Product;

public class CreateProductValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("اسم المنتج مطلوب.")
            .MaximumLength(100).WithMessage("اسم المنتج لا يتخطى 100 حرف.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("السعر لازم يكون أكبر من 0.");

        RuleFor(x => x.PurchasePrice)
            .GreaterThan(0).WithMessage("سعر الشراء لازم يكون أكبر من 0.");

        RuleFor(x => x.UnitType)
            .IsInEnum().WithMessage("وحدة القياس غير صحيحة.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("القسم مطلوب.");


        RuleFor(x => x.DiscountPercentage)
            .InclusiveBetween(0, 100).WithMessage("نسبة الخصم لازم تكون بين 0 و 100.")
            .When(x => x.DiscountPercentage.HasValue);

        RuleForEach(x => x.Images)
            .Must(ValidateFileSize).WithMessage("حجم الصورة كبير جداً، الحد الأقصى 2 ميجابايت.")
            .Must(ValidateExtension).WithMessage("امتداد الصورة غير مدعوم، يرجى رفع (jpg, jpeg, png, gif).")
            .When(x => x.Images != null && x.Images.Any());
    }

    private bool ValidateFileSize(Microsoft.AspNetCore.Http.IFormFile? file)
    {
        if (file == null) return true;
        return file.Length <= 2 * 1024 * 1024;
    }

    private bool ValidateExtension(Microsoft.AspNetCore.Http.IFormFile? file)
    {
        if (file == null) return true;
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return allowedExtensions.Contains(extension);
    }
}