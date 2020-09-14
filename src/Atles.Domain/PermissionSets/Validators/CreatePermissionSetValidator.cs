using Atles.Domain.PermissionSets.Commands;
using FluentValidation;

namespace Atles.Domain.PermissionSets.Validators
{
    public class CreatePermissionSetValidator : AbstractValidator<CreatePermissionSet>
    {
        public CreatePermissionSetValidator(IPermissionSetRules rules)
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Permission set name is required.")
                .Length(1, 50).WithMessage("Permission set name must be at least 1 and at max 50 characters long.")
                .MustAsync((c, p, cancellation) => rules.IsNameUniqueAsync(c.SiteId, p))
                    .WithMessage(c => $"A permission set with name {c.Name} already exists.");
        }
    }
}
