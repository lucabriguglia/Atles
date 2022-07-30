using Atles.Commands.Users;
using FluentValidation;

namespace Atles.Validators.Users
{
    public class CreateUserValidator : AbstractValidator<CreateUser>
    {
        public CreateUserValidator()
        {
            RuleFor(c => c.IdentityUserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email not valid.");
        }
    }
}
