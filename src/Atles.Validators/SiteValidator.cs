using Atles.Models.Admin.Sites;
using FluentValidation;

namespace Atles.Validators;

public class SiteValidator : AbstractValidator<SettingsPageModel.SiteModel>
{
    public SiteValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("Site title is required.")
            .Length(1, 50).WithMessage("Site title must be at least 1 and at max 50 characters long.");

        RuleFor(c => c.Theme)
            .NotEmpty().WithMessage("Site theme is required.")
            .Length(1, 250).WithMessage("Site theme must be at least 1 and at max 250 characters long.");

        RuleFor(c => c.Css)
            .NotEmpty().WithMessage("Site CSS is required.")
            .Length(1, 250).WithMessage("Site CSS must be at least 1 and at max 250 characters long.");

        RuleFor(c => c.Language)
            .NotEmpty().WithMessage("Site language is required.")
            .Length(1, 10).WithMessage("Site language must be at least 1 and at max 10 characters long.");

        RuleFor(c => c.Privacy)
            .NotEmpty().WithMessage("Site privacy is required.");

        RuleFor(c => c.Terms)
            .NotEmpty().WithMessage("Site terms is required.");
    }
}
