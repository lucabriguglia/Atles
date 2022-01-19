using Atles.Domain.Models.Categories.Commands;
using Atles.Domain.Models.Categories.Rules;
using Atles.Domain.Models.PermissionSets.Rules;
using Atles.Infrastructure;
using FluentValidation;

namespace Atles.Domain.Validators.Categories
{
    public class UpdateCategoryValidator : AbstractValidator<UpdateCategory>
    {
        public UpdateCategoryValidator(IDispatcher dispatcher)
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .Length(1, 50).WithMessage("Category name must be at least 1 and at max 50 characters long.")
                .MustAsync((c, p, cancellation) => dispatcher.Get(new IsCategoryNameUnique { SiteId = c.SiteId, Name = p, Id = c.Id }))
                    .WithMessage(c => $"A category with name {c.Name} already exists.");

            RuleFor(c => c.PermissionSetId)
                .MustAsync((c, p, cancellation) => dispatcher.Get(new IsPermissionSetValid { SiteId = c.SiteId, Id = p }))
                    .WithMessage(c => $"Permission set with id {c.PermissionSetId} does not exist.");
        }
    }
}
