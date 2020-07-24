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
                .Length(1, 100).WithMessage("Site title length must be between 1 and 100 characters.");
        }
    }
}
