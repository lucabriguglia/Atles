﻿using Atles.Commands.Categories;
using Atles.Validators.ValidationRules;
using FluentValidation;

namespace Atles.Validators.Categories;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategory>
{
    public UpdateCategoryValidator(ICategoryValidationRules categoryValidationRules, IPermissionSetValidationRules permissionSetValidationRules)
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .Length(1, 50).WithMessage("Category name must be at least 1 and at max 50 characters long.")
            .MustAsync(async (model, name, cancellation) => await categoryValidationRules.IsCategoryNameUnique(model.SiteId, name, model.CategoryId))
            .WithMessage(model => $"A category with name {model.Name} already exists.");

        RuleFor(c => c.PermissionSetId)
            .MustAsync(async (model, permissionSetId, cancellation) => await permissionSetValidationRules.IsPermissionSetValid(model.SiteId, permissionSetId))
            .WithMessage(model => $"Permission set with id {model.PermissionSetId} does not exist.");
    }
}