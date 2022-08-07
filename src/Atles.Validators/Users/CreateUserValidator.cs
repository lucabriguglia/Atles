using Atles.Models.Admin.Users;
using Atles.Validators.ValidationRules;
using FluentValidation;

namespace Atles.Validators.Users;

public class CreateUserValidator : AbstractValidator<CreatePageModel.UserModel>
{
    public CreateUserValidator(IUserValidationRules userValidationRules)
    {
        RuleFor(model => model.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email not valid.");

        RuleFor(model => model.Email)
            .MustAsync(UserEmailBeUnique).WithMessage(model => $"Email {model.Email} is already in use.");

        RuleFor(model => model.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Length(6, 100).WithMessage("Password must be at least 6 and at max 100 characters long.");

        RuleFor(model => model.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required.")
            .Equal(model => model.Password).WithMessage("Confirm password does not match password.");

        async Task<bool> UserEmailBeUnique(CreatePageModel.UserModel model, string email, CancellationToken cancellation) =>
            await userValidationRules.IsUserEmailUnique(Guid.Empty, email);
    }
}
