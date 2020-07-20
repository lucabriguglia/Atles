using Atlas.Domain.ForumGroups.Commands;
using FluentValidation;

namespace Atlas.Domain.ForumGroups.Validators
{
    public class CreateForumGroupValidator : AbstractValidator<CreateForumGroup>
    {
        public CreateForumGroupValidator(IForumGroupRules rules)
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Forum group name is required.")
                .Length(1, 100).WithMessage("Forum group name length must be between 1 and 100 characters.")
                .MustAsync((c, p, cancellation) => rules.IsNameUniqueAsync(c.SiteId, p)).WithMessage(c => $"A forum group with name {c.Name} already exists.");
        }
    }
}
