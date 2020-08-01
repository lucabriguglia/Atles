using Atlas.Domain.Members.Commands;
using FluentValidation;

namespace Atlas.Domain.Members.Validators
{
    public class CreateMemberValidator : AbstractValidator<CreateMember>
    {
        public CreateMemberValidator(IMemberRules rules)
        {
            RuleFor(c => c.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(c => c.DisplayName)
                .NotEmpty().WithMessage("Display name is required.")
                .Length(1, 50).WithMessage("Display name length must be between 1 and 50 characters.")
                .MustAsync((c, p, cancellation) => rules.IsDisplayNameUniqueAsync(p))
                    .WithMessage(c => $"A member with display name {c.DisplayName} already exists.");

            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email not valid.");
        }
    }
}
