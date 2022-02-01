using Atles.Domain.Commands;
using FluentValidation;

namespace Atles.Domain.Handlers.Posts.Commands.Validators
{
    public class UpdateReplyValidator : AbstractValidator<UpdateReply>
    {
        public UpdateReplyValidator()
        {
            RuleFor(c => c.Content)
                .NotEmpty().WithMessage("Reply content is required.");
        }
    }
}
