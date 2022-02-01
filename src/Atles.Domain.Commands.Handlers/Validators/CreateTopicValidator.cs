using Atles.Domain.Rules;
using Atles.Infrastructure;
using FluentValidation;

namespace Atles.Domain.Commands.Handlers.Validators
{
    public class CreateTopicValidator : AbstractValidator<CreateTopic>
    {
        public CreateTopicValidator(IDispatcher dispatcher)
        {
            RuleFor(c => c.Title)
                .NotEmpty().WithMessage("Topic title is required.")
                .Length(1, 100).WithMessage("Topic title must be at least 1 and at max 100 characters long.");

            RuleFor(c => c.Content)
                .NotEmpty().WithMessage("Topic content is required.");

            RuleFor(c => c.ForumId)
                .MustAsync((c, p, cancellation) => dispatcher.Get(new IsForumValid { SiteId = c.SiteId, Id = p }))
                    .WithMessage(c => $"Forum with id {c.ForumId} does not exist.");
        }
    }
}
