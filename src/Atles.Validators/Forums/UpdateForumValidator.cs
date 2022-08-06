using Atles.Models.Admin.Forums;
using Atles.Validators.ValidationRules;
using FluentValidation;

namespace Atles.Validators.Forums;

public class UpdateForumValidator : AbstractValidator<ForumFormModelBase.ForumModel>
{
    public UpdateForumValidator(IForumValidationRules forumValidationRules, IPermissionSetValidationRules permissionSetValidationRules)
    {
        RuleFor(model => model.Name)
            .NotEmpty().WithMessage("Forum name is required.")
            .Length(1, 50).WithMessage("Forum name must be at least 1 and at max 50 characters long.");

        RuleFor(model => model.Name)
            .MustAsync(ForumNameBeUnique).WithMessage(model => $"A forum with name {model.Name} already exists.")
            .When(model => !string.IsNullOrEmpty(model.Name) && model.Name.Length <= 50);

        RuleFor(model => model.Slug)
            .NotEmpty().WithMessage("Forum slug is required.")
            .Length(1, 50).WithMessage("Forum slug must be at least 1 and at max 50 characters long.");

        RuleFor(model => model.Slug)
            .MustAsync(ForumSlugBeUnique).WithMessage(model => $"A forum with slug {model.Slug} already exists.")
            .When(model => !string.IsNullOrEmpty(model.Slug) && model.Slug.Length <= 50);

        RuleFor(model => model.Description)
            .Length(1, 200).WithMessage("Forum description length must be between 1 and 200 characters.")
            .When(model => !string.IsNullOrWhiteSpace(model.Description));

        RuleFor(model => model.PermissionSetId)
            .MustAsync(PermissionSetBeValid).WithMessage(model => $"Permission set with id {model.PermissionSetId} does not exist.")
            .When(model => model.PermissionSetId != Guid.Empty);

        async Task<bool> ForumNameBeUnique(ForumFormModelBase.ForumModel model, string name, CancellationToken cancellation) =>
            await forumValidationRules.IsForumNameUnique(model.SiteId, model.CategoryId, name, model.Id);

        async Task<bool> ForumSlugBeUnique(ForumFormModelBase.ForumModel model, string slug, CancellationToken cancellation) =>
            await forumValidationRules.IsForumSlugUnique(model.SiteId, slug, model.Id);

        async Task<bool> PermissionSetBeValid(ForumFormModelBase.ForumModel model, Guid permissionSetId, CancellationToken cancellation) =>
            await permissionSetValidationRules.IsPermissionSetValid(model.SiteId, permissionSetId);
    }
}