using Atles.Models.Admin;
using Atles.Validators.ValidationRules;
using FluentValidation;

namespace Atles.Validators.Categories;

public class CreateCategoryValidator : AbstractValidator<CategoryFormModel.CategoryModel>
{
    public CreateCategoryValidator(ICategoryValidationRules categoryValidationRules, IPermissionSetValidationRules permissionSetValidationRules)
    {
        RuleFor(model => model.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .Length(1, 50).WithMessage("Category name must be at least 1 and at max 50 characters long.")
            .MustAsync(async (model, name, _) => await categoryValidationRules.IsCategoryNameUnique(model.SiteId, name))
            .WithMessage(model => $"A category with name {model.Name} already exists.");

        RuleFor(model => model.PermissionSetId)
            .MustAsync(async (model, permissionSetId, _) => await permissionSetValidationRules.IsPermissionSetValid(model.SiteId, permissionSetId))
            .WithMessage(model => $"Permission set with id {model.PermissionSetId} does not exist.");
    }
}