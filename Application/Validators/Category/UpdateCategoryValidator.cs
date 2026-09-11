using Application.DTOs.Category;
using FluentValidation;

namespace Application.Validators.Category
{
    public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryDto>
    {
        public UpdateCategoryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("معرف القسم (Id) مطلوب.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("اسم القسم مطلوب.")
                .MaximumLength(100).WithMessage("اسم القسم لا يمكن أن يتخطى 100 حرف.");

            // الـ Business Rule اللي بتمنع القسم يكون ابن لنفسه
            RuleFor(x => x)
                .Must(x => x.ParentCategoryId != x.Id)
                .WithName("ParentCategoryId")
                .WithMessage("لا يمكن للقسم أن يكون قسماً فرعياً لنفسه.");

            RuleFor(x => x.Image)
                .Must(ValidateFileSize).WithMessage("حجم الصورة كبير جداً، الحد الأقصى 2 ميجابايت.")
                .Must(ValidateExtension).WithMessage("امتداد الصورة غير مدعوم، يرجى رفع (jpg, jpeg, png, gif).")
                .When(x => x.Image != null);
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
            var extension = Path.GetExtension(file.FileName).ToLower();
            return allowedExtensions.Contains(extension);
        }
    }
}