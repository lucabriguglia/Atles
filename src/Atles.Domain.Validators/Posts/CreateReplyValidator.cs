using Atles.Domain.Forums.Rules;
using Atles.Domain.Posts.Commands;
using Atles.Domain.Posts.Rules;
using FluentValidation;
using OpenCqrs;

namespace Atles.Domain.Validators.Posts
{
    public class CreateReplyValidator : AbstractValidator<CreateReply>
    {
        public CreateReplyValidator(ISender sender)
        {
            RuleFor(c => c.Content)
                .NotEmpty().WithMessage("Reply content is required.");

            RuleFor(c => c.ForumId)
                .MustAsync((c, p, cancellation) => sender.Send(new IsForumValid { SiteId = c.SiteId, Id = p }))
                    .WithMessage(c => $"Forum with id {c.ForumId} does not exist.");

            RuleFor(c => c.TopicId)
                .MustAsync((c, p, cancellation) => sender.Send(new IsTopicValid { SiteId = c.SiteId, ForumId = c.ForumId, Id = p }))
                .WithMessage(c => $"Topic with id {c.ForumId} does not exist.");
        }
    }
}
