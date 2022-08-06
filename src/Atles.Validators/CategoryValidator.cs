using Atles.Models.Admin.Categories;
using Atles.Validators.ValidationRules;
using FluentValidation;

namespace Atles.Validators;

public class CategoryValidator : AbstractValidator<CategoryFormModel.CategoryModel>
{
    public CategoryValidator(ICategoryValidationRules categoryValidationRules, IPermissionSetValidationRules permissionSetValidationRules)
    {
        RuleFor(model => model.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .Length(1, 50).WithMessage("Category name must be at least 1 and at max 50 characters long.");

        RuleFor(model => model.Name)
            .MustAsync(CategoryNameBeUnique).WithMessage(model => $"A category with name {model.Name} already exists.")
            .When(model => !string.IsNullOrEmpty(model.Name) && model.Name.Length <= 50);

        RuleFor(model => model.PermissionSetId)
            .MustAsync(PermissionSetBeValid).WithMessage(model => $"Permission set with id {model.PermissionSetId} is not valid.");

        async Task<bool> CategoryNameBeUnique(CategoryFormModel.CategoryModel model, string name, CancellationToken cancellation) =>
            await categoryValidationRules.IsCategoryNameUnique(model.SiteId, model.Id, name);

        async Task<bool> PermissionSetBeValid(CategoryFormModel.CategoryModel model, Guid permissionSetId, CancellationToken cancellation) =>
            await permissionSetValidationRules.IsPermissionSetValid(model.SiteId, permissionSetId);
    }
}