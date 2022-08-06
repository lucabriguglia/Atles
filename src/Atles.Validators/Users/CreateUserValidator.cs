using Atles.Models.Admin.Users;
using FluentValidation;

namespace Atles.Validators.Users;

public class CreateUserValidator : AbstractValidator<CreatePageModel.UserModel>
{
    public CreateUserValidator()
    {
        RuleFor(model => model.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email not valid.");

        RuleFor(model => model.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Length(6, 100).WithMessage("Password must be at least 6 and at max 100 characters long.");

        RuleFor(model => model.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required.")
            .Equal(model => model.Password).WithMessage("Confirm password does not match password.");
    }
}
