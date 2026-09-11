using Application.DTOs.Review;
using FluentValidation;


namespace Application.Validators.Review
{
    public class UpdateReviewValidator : AbstractValidator<UpdateReviewDto>
    {
        public UpdateReviewValidator()
        {
            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("التقييم لازم يكون بين 1 و 5.");

            RuleFor(x => x.Comment)
                .MaximumLength(1000).WithMessage("التعليق لا يتخطى 1000 حرف.")
                .When(x => x.Comment != null);
        }
    }
}
