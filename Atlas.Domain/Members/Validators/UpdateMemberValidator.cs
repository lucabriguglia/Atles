using Atlas.Domain.Members.Commands;
using FluentValidation;

namespace Atlas.Domain.Members.Validators
{
    public class UpdateMemberValidator : AbstractValidator<UpdateMember>
    {
        public UpdateMemberValidator(IMemberRules rules)
        {
            RuleFor(c => c.DisplayName)
                .NotEmpty().WithMessage("Display name is required.")
                .Length(1, 50).WithMessage("Display name length must be between 1 and 50 characters.")
                .MustAsync((c, p, cancellation) => rules.IsDisplayNameUniqueAsync(p, c.Id))
                    .WithMessage(c => $"A member with display name {c.DisplayName} already exists.");
        }
    }
}
