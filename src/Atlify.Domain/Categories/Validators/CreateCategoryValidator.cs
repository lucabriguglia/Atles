using Atlify.Domain.Categories.Commands;
using Atlify.Domain.PermissionSets;
using FluentValidation;

namespace Atlify.Domain.Categories.Validators
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategory>
    {
        public CreateCategoryValidator(ICategoryRules rules, IPermissionSetRules permissionSetRules)
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .Length(1, 50).WithMessage("Category name must be at least 1 and at max 50 characters long.")
                .MustAsync((c, p, cancellation) => rules.IsNameUniqueAsync(c.SiteId, p))
                    .WithMessage(c => $"A category with name {c.Name} already exists.");

            RuleFor(c => c.PermissionSetId)
                .MustAsync((c, p, cancellation) => permissionSetRules.IsValidAsync(c.SiteId, p))
                    .WithMessage(c => $"Permission set with id {c.PermissionSetId} does not exist.");
        }
    }
}
