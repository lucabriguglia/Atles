using Atlas.Domain.Users.Commands;
using FluentValidation;

namespace Atlas.Domain.Users.Validators
{
    public class CreateUserValidator : AbstractValidator<CreateUser>
    {
        public CreateUserValidator(IUserRules rules)
        {
            RuleFor(c => c.IdentityUserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email not valid.");
        }
    }
}
