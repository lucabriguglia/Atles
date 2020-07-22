using Atlas.Domain.Forums.Commands;
using Atlas.Domain.PermissionSets;
using FluentValidation;

namespace Atlas.Domain.Forums.Validators
{
    public class CreateForumValidator : AbstractValidator<CreateForum>
    {
        public CreateForumValidator(IForumRules rules, IPermissionSetRules permissionSetRules)
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Forum name is required.")
                .Length(1, 100).WithMessage("Forum name length must be between 1 and 100 characters.")
                .MustAsync((c, p, cancellation) => rules.IsNameUniqueAsync(c.ForumGroupId, p))
                    .WithMessage(c => $"A forum with name {c.Name} already exists.");

            RuleFor(c => c.PermissionSetId)
                .MustAsync((c, p, cancellation) => permissionSetRules.IsValid(c.SiteId, p.Value))
                    .WithMessage(c => $"Permission set with id {c.PermissionSetId} does not exist.")
                    .When(c => c.PermissionSetId != null);
        }
    }
}
