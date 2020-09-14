using Atlas.Domain.Posts.Commands;
using FluentValidation;

namespace Atlas.Domain.Posts.Validators
{
    public class UpdateTopicValidator : AbstractValidator<UpdateTopic>
    {
        public UpdateTopicValidator()
        {
            RuleFor(c => c.Title)
                .NotEmpty().WithMessage("Topic title is required.")
                .Length(1, 100).WithMessage("Topic title must be at least 1 and at max 50 characters long.");

            RuleFor(c => c.Slug)
                .Length(1, 50).WithMessage("Topic slug must be at max 50 characters long.")
                .When(c => !string.IsNullOrWhiteSpace(c.Slug));

            RuleFor(c => c.Content)
                .NotEmpty().WithMessage("Topic content is required.");
        }
    }
}
