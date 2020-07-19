using Atlas.Server.Domain.Commands;
using FluentValidation;

namespace Atlas.Server.Domain.ForumGroups.Validators
{
    public class CreateForumGroupValidator : AbstractValidator<CreateForumGroup>
    {
        public CreateForumGroupValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Forum group name is required.")
                .Length(1, 100).WithMessage("Forum group name length must be between 1 and 100 characters.");
        }
    }
}
