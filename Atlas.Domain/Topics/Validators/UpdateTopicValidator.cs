using Atlas.Domain.Topics.Commands;
using FluentValidation;

namespace Atlas.Domain.Topics.Validators
{
    public class UpdateTopicValidator : AbstractValidator<UpdateTopic>
    {
        public UpdateTopicValidator()
        {
            RuleFor(c => c.Title)
                .NotEmpty().WithMessage("Topic title is required.")
                .Length(1, 100).WithMessage("Topic title length must be between 1 and 100 characters.");

            RuleFor(c => c.Content)
                .NotEmpty().WithMessage("Topic content is required.");
        }
    }
}
