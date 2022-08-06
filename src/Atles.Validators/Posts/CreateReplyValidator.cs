using Atles.Commands.Posts;
using Atles.Validators.ValidationRules;
using FluentValidation;

namespace Atles.Validators.Posts;

public class CreateReplyValidator : AbstractValidator<CreateReply>
{
    public CreateReplyValidator(IForumValidationRules forumValidationRules, ITopicValidationRules topicValidationRules)
    {
        RuleFor(c => c.Content)
            .NotEmpty().WithMessage("Reply content is required.");

        RuleFor(c => c.ForumId)
            .MustAsync(async (model, forumId, cancellation) => await forumValidationRules.IsForumValid(model.SiteId, forumId))
            .WithMessage(c => $"Forum with id {c.ForumId} does not exist.");

        RuleFor(c => c.TopicId)
            .MustAsync(async (model, topicId, cancellation) => await topicValidationRules.IsTopicValid(model.SiteId, model.ForumId, topicId))
            .WithMessage(c => $"Topic with id {c.ForumId} does not exist.");
    }
}
