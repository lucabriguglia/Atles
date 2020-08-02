using Atlas.Domain.Sites.Commands;
using FluentValidation;

namespace Atlas.Domain.Sites.Validators
{
    public class UpdateSiteValidator : AbstractValidator<UpdateSite>
    {
        public UpdateSiteValidator()
        {
            RuleFor(c => c.Title)
                .NotEmpty().WithMessage("Site title is required.")
                .Length(1, 100).WithMessage("Site title must be at least 1 and at max 50 characters long.");
        }
    }
}
