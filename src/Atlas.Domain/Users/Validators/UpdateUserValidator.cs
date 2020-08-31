using Atlas.Domain.Users.Commands;
using FluentValidation;

namespace Atlas.Domain.Users.Validators
{
    public class UpdateUserValidator : AbstractValidator<UpdateUser>
    {
        public UpdateUserValidator(IUserRules rules)
        {
            RuleFor(c => c.DisplayName)
                .NotEmpty().WithMessage("Display name is required.")
                .Length(1, 50).WithMessage("Display name must be at least 1 and at max 50 characters long.")
                .MustAsync((c, p, cancellation) => rules.IsDisplayNameUniqueAsync(p, c.Id))
                    .WithMessage(c => $"A user with display name {c.DisplayName} already exists.");
        }
    }
}
