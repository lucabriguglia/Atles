using Atles.Domain.Forums.Rules;
using Atles.Domain.Posts.Commands;
using Atles.Infrastructure.Queries;
using FluentValidation;

namespace Atles.Domain.Posts.Validators
{
    public class CreateTopicValidator : AbstractValidator<CreateTopic>
    {
        public CreateTopicValidator(IQuerySender queries)
        {
            RuleFor(c => c.Title)
                .NotEmpty().WithMessage("Topic title is required.")
                .Length(1, 100).WithMessage("Topic title must be at least 1 and at max 100 characters long.");

            RuleFor(c => c.Slug)
                .Length(1, 50).WithMessage("Topic slug must be at max 50 characters long.")
                .When(c => !string.IsNullOrWhiteSpace(c.Slug));

            RuleFor(c => c.Content)
                .NotEmpty().WithMessage("Topic content is required.");

            RuleFor(c => c.ForumId)
                .MustAsync((c, p, cancellation) => queries.Send(new IsForumValid { SiteId = c.SiteId, Id = p }))
                    .WithMessage(c => $"Forum with id {c.ForumId} does not exist.");
        }
    }
}
