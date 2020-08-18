using Atlify.Domain.Members.Commands;
using FluentValidation;

namespace Atlify.Domain.Members.Validators
{
    public class CreateMemberValidator : AbstractValidator<CreateMember>
    {
        public CreateMemberValidator(IMemberRules rules)
        {
            RuleFor(c => c.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email not valid.");
        }
    }
}
