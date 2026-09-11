using Application.DTOs.Category;
using FluentValidation;

namespace Application.Validators.Category
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("اسم القسم مطلوب.")
                .MaximumLength(100).WithMessage("اسم القسم لا يمكن أن يتخطى 100 حرف.");

            // التحقق من ملف الصورة لو موجود
            RuleFor(x => x.Image)
                .Must(ValidateFileSize).WithMessage("حجم الصورة كبير جداً، الحد الأقصى 2 ميجابايت.")
                .Must(ValidateExtension).WithMessage("امتداد الصورة غير مدعوم، يرجى رفع (jpg, jpeg, png, gif).")
                .When(x => x.Image != null); // يطبق فقط لو فيه صورة مبعوتة
        }

        private bool ValidateFileSize(Microsoft.AspNetCore.Http.IFormFile? file)
        {
            if (file == null) return true;
            // 2 Megabytes = 2 * 1024 * 1024 Bytes
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
}