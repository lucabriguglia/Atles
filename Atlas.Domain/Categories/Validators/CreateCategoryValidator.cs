using Atlas.Domain.Categories.Commands;
using Atlas.Domain.PermissionSets;
using FluentValidation;

namespace Atlas.Domain.Categories.Validators
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategory>
    {
        public CreateCategoryValidator(ICategoryRules rules, IPermissionSetRules permissionSetRules)
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .Length(1, 50).WithMessage("Category name length must be between 1 and 50 characters.")
                .MustAsync((c, p, cancellation) => rules.IsNameUniqueAsync(c.SiteId, p))
                    .WithMessage(c => $"A category with name {c.Name} already exists.");

            RuleFor(c => c.PermissionSetId)
                .MustAsync((c, p, cancellation) => permissionSetRules.IsValid(c.SiteId, p))
                    .WithMessage(c => $"Permission set with id {c.PermissionSetId} does not exist.");
        }
    }
}
