using Atles.Commands.Posts;
using Atles.Core;
using Atles.Domain.Rules.Forums;
using Atles.Domain.Rules.Posts;
using FluentValidation;

namespace Atles.Validators.Posts
{
    public class CreateReplyValidator : AbstractValidator<CreateReply>
    {
        public CreateReplyValidator(IDispatcher dispatcher)
        {
            RuleFor(c => c.Content)
                .NotEmpty().WithMessage("Reply content is required.");

            RuleFor(c => c.ForumId)
                .MustAsync(async (c, p, cancellation) =>
                {
                    // TODO: To be moved to a service
                    var result = await dispatcher.Get(new IsForumValid {SiteId = c.SiteId, Id = p});
                    return result.AsT0;
                })
                    .WithMessage(c => $"Forum with id {c.ForumId} does not exist.");

            RuleFor(c => c.TopicId)
                .MustAsync(async (c, p, cancellation) =>
                {
                    // TODO: To be moved to a service
                    var result = await dispatcher.Get(new IsTopicValid {SiteId = c.SiteId, ForumId = c.ForumId, Id = p});
                    return result.AsT0;
                })
                .WithMessage(c => $"Topic with id {c.ForumId} does not exist.");
        }
    }
}
