using Atles.Commands.Forums;
using Atles.Validators.PermissionSets;
using FluentValidation;

namespace Atles.Validators.Forums
{
    public class UpdateForumValidator : AbstractValidator<UpdateForum>
    {
        public UpdateForumValidator(IForumValidationRules forumValidationRules, IPermissionSetValidationRules permissionSetValidationRules)
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Forum name is required.")
                .Length(1, 50).WithMessage("Forum name must be at least 1 and at max 50 characters long.")
                .MustAsync(async (model, name, cancellation) => await forumValidationRules.IsForumNameUnique(model.SiteId, model.CategoryId, name, model.ForumId))
                .WithMessage(c => $"A forum with name {c.Name} already exists.");

            RuleFor(c => c.Slug)
                .NotEmpty().WithMessage("Forum slug is required.")
                .Length(1, 50).WithMessage("Forum slug must be at least 1 and at max 50 characters long.")
                .MustAsync(async (model, slug, cancellation) => await forumValidationRules.IsForumSlugUnique(model.SiteId, model.CategoryId, slug, model.ForumId))
                .WithMessage(c => $"A forum with slug {c.Slug} already exists.");

            RuleFor(c => c.Description)
                .Length(1, 200).WithMessage("Forum description length must be between 1 and 200 characters.")
                .When(c => !string.IsNullOrWhiteSpace(c.Description));

            RuleFor(c => c.PermissionSetId)
                .MustAsync(async (model, permissionSetId, cancellation) => await permissionSetValidationRules.IsPermissionSetValid(model.SiteId, permissionSetId.Value))
                .WithMessage(model => $"Permission set with id {model.PermissionSetId} does not exist.")
                .When(model => model.PermissionSetId != null);
        }
    }
}
