using Atles.Domain.Models.Posts.Commands;
using FluentValidation;

namespace Atles.Domain.Handlers.Posts.Commands.Validators
{
    public class UpdateTopicValidator : AbstractValidator<UpdateTopic>
    {
        public UpdateTopicValidator()
        {
            RuleFor(c => c.Title)
                .NotEmpty().WithMessage("Topic title is required.")
                .Length(1, 100).WithMessage("Topic title must be at least 1 and at max 50 characters long.");

            RuleFor(c => c.Content)
                .NotEmpty().WithMessage("Topic content is required.");
        }
    }
}
