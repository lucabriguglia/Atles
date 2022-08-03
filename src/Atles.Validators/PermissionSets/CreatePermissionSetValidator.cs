using Atles.Commands.PermissionSets;
using Atles.Validators.ValidationRules;
using FluentValidation;

namespace Atles.Validators.PermissionSets;

public class CreatePermissionSetValidator : AbstractValidator<CreatePermissionSet>
{
    public CreatePermissionSetValidator(IPermissionSetValidationRules permissionSetValidationRules)
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Permission set name is required.")
            .Length(1, 50).WithMessage("Permission set name must be at least 1 and at max 50 characters long.")
            .MustAsync(async (model, name, cancellation) => await permissionSetValidationRules.IsPermissionSetNameUnique(model.SiteId, model.Name))
            .WithMessage(c => $"A permission set with name {c.Name} already exists.");
    }
}
