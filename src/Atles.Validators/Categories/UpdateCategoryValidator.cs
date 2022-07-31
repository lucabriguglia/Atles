using Atles.Commands.Categories;
using Atles.Core;
using Atles.Domain.Rules.Categories;
using Atles.Domain.Rules.PermissionSets;
using FluentValidation;

namespace Atles.Validators.Categories
{
    public class UpdateCategoryValidator : AbstractValidator<UpdateCategory>
    {
        public UpdateCategoryValidator(IDispatcher dispatcher)
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .Length(1, 50).WithMessage("Category name must be at least 1 and at max 50 characters long.")
                .MustAsync(async (c, p, cancellation) =>
                {
                    // TODO: To be moved to a service
                    var result = await dispatcher.Get(new IsCategoryNameUnique
                            {SiteId = c.SiteId, Name = p, Id = c.CategoryId});
                    return result.AsT0;
                })
                    .WithMessage(c => $"A category with name {c.Name} already exists.");

            RuleFor(c => c.PermissionSetId)
                .MustAsync(async (c, p, cancellation) =>
                {
                    // TODO: To be moved to a service
                    var result = await dispatcher.Get(new IsPermissionSetValid {SiteId = c.SiteId, Id = p});
                    return result.AsT0;
                })
                    .WithMessage(c => $"Permission set with id {c.PermissionSetId} does not exist.");
        }
    }
}
