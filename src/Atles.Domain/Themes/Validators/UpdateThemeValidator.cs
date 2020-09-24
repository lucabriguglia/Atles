using Atles.Domain.Themes.Commands;
using FluentValidation;

namespace Atles.Domain.Themes.Validators
{
    /// <summary>
    /// Validate a request to update a Theme.
    /// </summary>
    public class UpdateThemeValidator : AbstractValidator<UpdateTheme>
    {
        /// <summary>
        /// UpdateThemeValidator
        /// </summary>
        /// <param name="rules"></param>
        public UpdateThemeValidator(IThemeRules rules)
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Length(1, 100).WithMessage("Name must be at least 1 and at max 50 characters long.")
                .MustAsync((c, p, cancellation) => rules.IsNameUniqueAsync(c.SiteId, p, c.Id))
                .WithMessage(c => $"Name {c.Name} is already in use.");
        }
    }
}