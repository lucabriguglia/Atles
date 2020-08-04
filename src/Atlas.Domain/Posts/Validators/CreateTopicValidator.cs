using Atlas.Domain.Forums;
using Atlas.Domain.Posts.Commands;
using FluentValidation;

namespace Atlas.Domain.Posts.Validators
{
    public class CreateTopicValidator : AbstractValidator<CreateTopic>
    {
        public CreateTopicValidator(IForumRules forumRules)
        {
            RuleFor(c => c.Title)
                .NotEmpty().WithMessage("Topic title is required.")
                .Length(1, 100).WithMessage("Topic title must be at least 1 and at max 50 characters long.");

            RuleFor(c => c.Content)
                .NotEmpty().WithMessage("Topic content is required.");

            RuleFor(c => c.ForumId)
                .MustAsync((c, p, cancellation) => forumRules.IsValidAsync(c.SiteId, p))
                    .WithMessage(c => $"Forum with id {c.ForumId} does not exist.");
        }
    }
}
