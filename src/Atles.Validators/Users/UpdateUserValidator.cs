using Atles.Commands.Users;
using FluentValidation;

namespace Atles.Validators.Users;

public class UpdateUserValidator : AbstractValidator<UpdateUser>
{
    public UpdateUserValidator(IUserValidationRules userValidationRules)
    {
        RuleFor(c => c.DisplayName)
            .NotEmpty().WithMessage("Display name is required.")
            .Length(1, 50).WithMessage("Display name must be at least 1 and at max 50 characters long.")
            .MustAsync(async (c, p, cancellation) => await userValidationRules.IsUserDisplayNameUnique(c.DisplayName, c.UserId))
            .WithMessage(c => $"A user with display name {c.DisplayName} already exists.");
    }
}
