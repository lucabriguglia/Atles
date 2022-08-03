using Atles.Commands.Posts;
using Atles.Core;
using Atles.Validators.ValidationRules;
using FluentValidation;

namespace Atles.Validators.Posts
{
    public class CreateTopicValidator : AbstractValidator<CreateTopic>
    {
        public CreateTopicValidator(IDispatcher dispatcher, IForumValidationRules forumValidationRules)
        {
            RuleFor(c => c.Title)
                .NotEmpty().WithMessage("Topic title is required.")
                .Length(1, 100).WithMessage("Topic title must be at least 1 and at max 100 characters long.");

            RuleFor(c => c.Content)
                .NotEmpty().WithMessage("Topic content is required.");

            RuleFor(c => c.ForumId)
                .MustAsync(async (model, forumId, cancellation) => await forumValidationRules.IsForumValid(model.SiteId, forumId))
                    .WithMessage(c => $"Forum with id {c.ForumId} does not exist.");
        }
    }
}
