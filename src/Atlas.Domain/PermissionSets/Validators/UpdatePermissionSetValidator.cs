using Atlas.Domain.PermissionSets.Commands;
using FluentValidation;

namespace Atlas.Domain.PermissionSets.Validators
{
    public class UpdatePermissionSetValidator : AbstractValidator<UpdatePermissionSet>
    {
        public UpdatePermissionSetValidator(IPermissionSetRules rules)
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Permission set name is required.")
                .Length(1, 50).WithMessage("Permission set name must be at least 1 and at max 50 characters long.")
                .MustAsync((c, p, cancellation) => rules.IsNameUniqueAsync(c.SiteId, p, c.Id))
                    .WithMessage(c => $"A permission set with name {c.Name} already exists.");
        }
    }
}
