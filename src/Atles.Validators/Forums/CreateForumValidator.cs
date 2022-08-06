using Atles.Models.Admin.Forums;
using Atles.Validators.ValidationRules;
using FluentValidation;

namespace Atles.Validators.Forums;

public class CreateForumValidator : AbstractValidator<CreateForumFormModel.ForumModel>
{
    public CreateForumValidator(IForumValidationRules forumValidationRules, IPermissionSetValidationRules permissionSetValidationRules)
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Forum name is required.")
            .Length(1, 50).WithMessage("Forum name must be at least 1 and at max 50 characters long.");

        RuleFor(c => c.Name)
            .MustAsync(ForumNameBeUnique)
            .WithMessage(c => $"A forum with name {c.Name} already exists.")
            .When(model => !string.IsNullOrEmpty(model.Name) && model.Name.Length <= 50);

        RuleFor(c => c.Slug)
            .NotEmpty().WithMessage("Forum slug is required.")
            .Length(1, 50).WithMessage("Forum slug must be at least 1 and at max 50 characters long.");

        RuleFor(c => c.Slug)
            .MustAsync(ForumSlugBeUnique)
            .WithMessage(c => $"A forum with slug {c.Slug} already exists.");

        RuleFor(c => c.Description)
            .Length(1, 200).WithMessage("Forum description length must be between 1 and 200 characters.")
            .When(c => !string.IsNullOrWhiteSpace(c.Description));

        RuleFor(c => c.PermissionSetId)
            .MustAsync(PermissionSetBeValid)
            .WithMessage(model => $"Permission set with id {model.PermissionSetId} does not exist.")
            .When(model => model.PermissionSetId != null);

        async Task<bool> ForumNameBeUnique(CreateForumFormModel.ForumModel model, string name, CancellationToken cancellation) =>
            await forumValidationRules.IsForumNameUnique(model.SiteId, model.CategoryId, name);

        async Task<bool> ForumSlugBeUnique(CreateForumFormModel.ForumModel model, string slug, CancellationToken cancellation) =>
            await forumValidationRules.IsForumSlugUnique(model.SiteId, slug);

        async Task<bool> PermissionSetBeValid(CreateForumFormModel.ForumModel model, Guid? permissionSetId, CancellationToken cancellation) =>
            await permissionSetValidationRules.IsPermissionSetValid(model.SiteId, permissionSetId.Value);
    }
}