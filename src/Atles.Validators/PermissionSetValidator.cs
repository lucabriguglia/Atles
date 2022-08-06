using Atles.Models.Admin.PermissionSets;
using Atles.Validators.ValidationRules;
using FluentValidation;

namespace Atles.Validators;

public class PermissionSetValidator : AbstractValidator<PermissionSetFormModel.PermissionSetModel>
{
    public PermissionSetValidator(IPermissionSetValidationRules permissionSetValidationRules)
    {
        RuleFor(model => model.Name)
            .NotEmpty().WithMessage("Permission set name is required.")
            .Length(1, 50).WithMessage("Permission set name must be at least 1 and at max 50 characters long.");

        RuleFor(model => model.Name)
            .MustAsync(PermissionSetNameBeUnique).WithMessage(model => $"A permission set with name {model.Name} already exists.")
            .When(model => !string.IsNullOrEmpty(model.Name) && model.Name.Length <= 50);

        async Task<bool> PermissionSetNameBeUnique(PermissionSetFormModel.PermissionSetModel model, string name, CancellationToken cancellation) =>
            await permissionSetValidationRules.IsPermissionSetNameUnique(model.SiteId, model.Id, name);
    }
}
