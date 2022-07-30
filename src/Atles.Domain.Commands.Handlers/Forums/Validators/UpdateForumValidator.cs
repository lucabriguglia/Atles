using Atles.Commands.Forums;
using Atles.Core;
using Atles.Domain.Rules.Forums;
using Atles.Domain.Rules.PermissionSets;
using FluentValidation;

namespace Atles.Commands.Handlers.Forums.Validators
{
    public class UpdateForumValidator : AbstractValidator<UpdateForum>
    {
        public UpdateForumValidator(IDispatcher dispatcher)
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Forum name is required.")
                .Length(1, 50).WithMessage("Forum name must be at least 1 and at max 50 characters long.")
                .MustAsync((c, p, cancellation) => dispatcher.Get(new IsForumNameUnique { SiteId = c.SiteId, CategoryId = c.CategoryId, Name = p, Id = c.ForumId }))
                    .WithMessage(c => $"A forum with name {c.Name} already exists.");

            RuleFor(c => c.Slug)
                .NotEmpty().WithMessage("Forum slug is required.")
                .Length(1, 50).WithMessage("Forum slug must be at least 1 and at max 50 characters long.")
                .MustAsync((c, p, cancellation) => dispatcher.Get(new IsForumSlugUnique { SiteId = c.SiteId, Slug = p, Id = c.ForumId }))
                .WithMessage(c => $"A forum with slug {c.Slug} already exists.");

            RuleFor(c => c.Description)
                .Length(1, 200).WithMessage("Forum description length must be between 1 and 200 characters.")
                .When(c => !string.IsNullOrWhiteSpace(c.Description));

            RuleFor(c => c.PermissionSetId)
                .MustAsync((c, p, cancellation) => dispatcher.Get(new IsPermissionSetValid { SiteId = c.SiteId, Id = p.Value }))
                    .WithMessage(c => $"Permission set with id {c.PermissionSetId} does not exist.")
                    .When(c => c.PermissionSetId != null);
        }
    }
}
