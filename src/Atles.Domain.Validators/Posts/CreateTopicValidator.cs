using Atles.Domain.Forums.Rules;
using Atles.Domain.Posts.Commands;
using FluentValidation;
using OpenCqrs;

namespace Atles.Domain.Validators.Posts
{
    public class CreateTopicValidator : AbstractValidator<CreateTopic>
    {
        public CreateTopicValidator(ISender sender)
        {
            RuleFor(c => c.Title)
                .NotEmpty().WithMessage("Topic title is required.")
                .Length(1, 100).WithMessage("Topic title must be at least 1 and at max 100 characters long.");

            RuleFor(c => c.Slug)
                .NotEmpty().WithMessage("Topic slug is required.")
                .Length(1, 50).WithMessage("Topic slug must be at max 50 characters long.");

            RuleFor(c => c.Content)
                .NotEmpty().WithMessage("Topic content is required.");

            RuleFor(c => c.ForumId)
                .MustAsync((c, p, cancellation) => sender.Send(new IsForumValid { SiteId = c.SiteId, Id = p }))
                    .WithMessage(c => $"Forum with id {c.ForumId} does not exist.");
        }
    }
}
