using Atles.Domain.Commands;
using Atles.Domain.Rules;
using Atles.Infrastructure;
using FluentValidation;

namespace Atles.Domain.Handlers.Posts.Commands.Validators
{
    public class CreateReplyValidator : AbstractValidator<CreateReply>
    {
        public CreateReplyValidator(IDispatcher dispatcher)
        {
            RuleFor(c => c.Content)
                .NotEmpty().WithMessage("Reply content is required.");

            RuleFor(c => c.ForumId)
                .MustAsync((c, p, cancellation) => dispatcher.Get(new IsForumValid { SiteId = c.SiteId, Id = p }))
                    .WithMessage(c => $"Forum with id {c.ForumId} does not exist.");

            RuleFor(c => c.TopicId)
                .MustAsync((c, p, cancellation) => dispatcher.Get(new IsTopicValid { SiteId = c.SiteId, ForumId = c.ForumId, Id = p }))
                .WithMessage(c => $"Topic with id {c.ForumId} does not exist.");
        }
    }
}
