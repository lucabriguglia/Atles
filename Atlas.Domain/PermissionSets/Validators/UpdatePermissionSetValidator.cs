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
                .Length(1, 50).WithMessage("Permission set name length must be between 1 and 50 characters.")
                .MustAsync((c, p, cancellation) => rules.IsNameUniqueAsync(c.SiteId, p, c.Id))
                    .WithMessage(c => $"A permission set with name {c.Name} already exists.");
        }
    }
}
