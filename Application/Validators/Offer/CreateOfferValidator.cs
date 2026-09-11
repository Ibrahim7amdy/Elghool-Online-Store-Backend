using Application.DTOs.Offer;
using FluentValidation;

namespace Application.Validators.Offer;

public class CreateOfferValidator : AbstractValidator<CreateOfferDto>
{
    public CreateOfferValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("عنوان العرض مطلوب.")
            .MaximumLength(200).WithMessage("عنوان العرض لا يتخطى 200 حرف.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("الوصف لا يتخطى 1000 حرف.")
            .When(x => x.Description != null);

        RuleFor(x => x.DiscountPercentage)
            .InclusiveBetween(1, 100).WithMessage("نسبة الخصم لازم تكون بين 1 و 100.")
            .When(x => x.DiscountPercentage.HasValue);

        RuleFor(x => x.BundlePrice)
            .GreaterThan(0).WithMessage("سعر الباقة لازم يكون أكبر من 0.")
            .When(x => x.BundlePrice.HasValue);

        RuleFor(x => x)
            .Must(x => x.DiscountPercentage.HasValue || x.BundlePrice.HasValue)
            .WithMessage("لازم تحدد إما نسبة خصم أو سعر باقة.")
            .Must(x => !(x.DiscountPercentage.HasValue && x.BundlePrice.HasValue))
            .WithMessage("متقدرش تحدد نسبة خصم وسعر باقة في نفس الوقت.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("تاريخ البداية مطلوب.")
            .LessThan(x => x.EndDate).WithMessage("تاريخ البداية لازم يكون قبل تاريخ النهاية.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("تاريخ النهاية مطلوب.")
            .GreaterThan(DateTime.UtcNow).WithMessage("تاريخ النهاية لازم يكون في المستقبل.");

        RuleFor(x => x.Image)
            .Must(file => file!.Length <= 2 * 1024 * 1024)
                .WithMessage("حجم الصورة كبير جداً، الحد الأقصى 2 ميجابايت.")
            .Must(file => new[] { ".jpg", ".jpeg", ".png", ".gif" }
                .Contains(Path.GetExtension(file!.FileName).ToLowerInvariant()))
                .WithMessage("امتداد الصورة غير مدعوم.")
            .When(x => x.Image != null);
    }
}

