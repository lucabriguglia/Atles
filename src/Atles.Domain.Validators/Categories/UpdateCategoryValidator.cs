using Atles.Domain.Categories.Commands;
using Atles.Domain.Categories.Rules;
using Atles.Domain.PermissionSets.Rules;
using FluentValidation;
using OpenCqrs;

namespace Atles.Domain.Validators.Categories
{
    public class UpdateCategoryValidator : AbstractValidator<UpdateCategory>
    {
        public UpdateCategoryValidator(ISender sender)
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .Length(1, 50).WithMessage("Category name must be at least 1 and at max 50 characters long.")
                .MustAsync((c, p, cancellation) => sender.Send(new IsCategoryNameUnique { SiteId = c.SiteId, Name = p, Id = c.Id }))
                    .WithMessage(c => $"A category with name {c.Name} already exists.");

            RuleFor(c => c.PermissionSetId)
                .MustAsync((c, p, cancellation) => sender.Send(new IsPermissionSetValid { SiteId = c.SiteId, Id = p }))
                    .WithMessage(c => $"Permission set with id {c.PermissionSetId} does not exist.");
        }
    }
}
