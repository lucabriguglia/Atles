using Atles.Domain.Forums;
using Atles.Domain.Posts.Commands;
using FluentValidation;

namespace Atles.Domain.Posts.Validators
{
    public class CreateReplyValidator : AbstractValidator<CreateReply>
    {
        public CreateReplyValidator(IForumRules forumRules, ITopicRules topicRules)
        {
            RuleFor(c => c.Content)
                .NotEmpty().WithMessage("Reply content is required.");

            RuleFor(c => c.ForumId)
                .MustAsync((c, p, cancellation) => forumRules.IsValidAsync(c.SiteId, p))
                    .WithMessage(c => $"Forum with id {c.ForumId} does not exist.");

            RuleFor(c => c.TopicId)
                .MustAsync((c, p, cancellation) => topicRules.IsValidAsync(c.SiteId, c.ForumId, p))
                .WithMessage(c => $"Topic with id {c.ForumId} does not exist.");
        }
    }
}
