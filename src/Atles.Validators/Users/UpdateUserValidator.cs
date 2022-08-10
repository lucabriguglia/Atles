using Atles.Models.Admin.Users;
using Atles.Validators.ValidationRules;
using FluentValidation;

namespace Atles.Validators.Users;

public class UpdateUserValidator : AbstractValidator<EditUserPageModel.UserModel>
{
    public UpdateUserValidator(IUserValidationRules userValidationRules)
    {
        RuleFor(c => c.DisplayName)
            .NotEmpty().WithMessage("Display name is required.")
            .Length(1, 50).WithMessage("Display name must be at least 1 and at max 50 characters long.");

        RuleFor(c => c.DisplayName)
            .MustAsync(UserDisplayNameBeUnique).WithMessage(c => $"A user with display name {c.DisplayName} already exists.")
            .When(model => !string.IsNullOrEmpty(model.DisplayName) && model.DisplayName.Length < 50);

        async Task<bool> UserDisplayNameBeUnique(EditUserPageModel.UserModel model, string displayName, CancellationToken cancellation) =>
            await userValidationRules.IsUserDisplayNameUnique(model.Id, displayName);
    }
}
