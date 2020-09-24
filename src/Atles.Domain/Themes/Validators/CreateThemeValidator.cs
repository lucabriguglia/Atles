using Atles.Domain.Themes.Commands;
using FluentValidation;

namespace Atles.Domain.Themes.Validators
{
    /// <summary>
    /// Validate a request to create a new Theme.
    /// </summary>
    public class CreateThemeValidator : AbstractValidator<CreateTheme>
    {
        /// <summary>
        /// CreateThemeValidator
        /// </summary>
        /// <param name="rules"></param>
        public CreateThemeValidator(IThemeRules rules)
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Length(1, 100).WithMessage("Name must be at least 1 and at max 50 characters long.")
                .MustAsync((c, p, cancellation) => rules.IsNameUniqueAsync(c.SiteId, p))
                .WithMessage(c => $"Name {c.Name} is already in use.");
        }
    }
}