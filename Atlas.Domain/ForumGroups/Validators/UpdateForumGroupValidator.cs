using Atlas.Domain.ForumGroups.Commands;
using Atlas.Domain.PermissionSets;
using FluentValidation;

namespace Atlas.Domain.ForumGroups.Validators
{
    public class UpdateForumGroupValidator : AbstractValidator<UpdateForumGroup>
    {
        public UpdateForumGroupValidator(IForumGroupRules rules, IPermissionSetRules permissionSetRules)
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Forum group name is required.")
                .Length(1, 100).WithMessage("Forum group name length must be between 1 and 100 characters.")
                .MustAsync((c, p, cancellation) => rules.IsNameUniqueAsync(c.SiteId, c.Id, p)).WithMessage(c => $"A forum group with name {c.Name} already exists.");

            RuleFor(c => c.PermissionSetId)
                .MustAsync((c, p, cancellation) => permissionSetRules.IsValid(c.SiteId, p.Value))
                .WithMessage(c => $"Permission set with id {c.PermissionSetId} does not exist.")
                .When(c => c.PermissionSetId != null);
        }
    }
}
