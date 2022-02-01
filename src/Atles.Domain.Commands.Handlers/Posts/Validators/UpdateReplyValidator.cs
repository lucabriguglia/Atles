using Atles.Domain.Commands.Posts;
using FluentValidation;

namespace Atles.Domain.Commands.Handlers.Posts.Validators
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
