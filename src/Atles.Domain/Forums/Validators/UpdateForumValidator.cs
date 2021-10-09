using Atles.Domain.Forums.Commands;
using Atles.Domain.Forums.Rules;
using Atles.Domain.PermissionSets.Rules;
using FluentValidation;
using OpenCqrs.Queries;

namespace Atles.Domain.Forums.Validators
{
    public class UpdateForumValidator : AbstractValidator<UpdateForum>
    {
        public UpdateForumValidator(IQuerySender queries)
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Forum name is required.")
                .Length(1, 50).WithMessage("Forum name must be at least 1 and at max 50 characters long.")
                .MustAsync((c, p, cancellation) => queries.Send(new IsForumNameUnique { SiteId = c.SiteId, CategoryId = c.CategoryId, Name = p, Id = c.Id }))
                    .WithMessage(c => $"A forum with name {c.Name} already exists.");

            RuleFor(c => c.Slug)
                .NotEmpty().WithMessage("Forum slug is required.")
                .Length(1, 50).WithMessage("Forum slug must be at least 1 and at max 50 characters long.")
                .MustAsync((c, p, cancellation) => queries.Send(new IsForumSlugUnique { SiteId = c.SiteId, Slug = p, Id = c.Id }))
                .WithMessage(c => $"A forum with slug {c.Slug} already exists.");

            RuleFor(c => c.Description)
                .Length(1, 200).WithMessage("Forum description length must be between 1 and 200 characters.")
                .When(c => !string.IsNullOrWhiteSpace(c.Description));

            RuleFor(c => c.PermissionSetId)
                .MustAsync((c, p, cancellation) => queries.Send(new IsPermissionSetValid { SiteId = c.SiteId, Id = p.Value }))
                    .WithMessage(c => $"Permission set with id {c.PermissionSetId} does not exist.")
                    .When(c => c.PermissionSetId != null);
        }
    }
}
