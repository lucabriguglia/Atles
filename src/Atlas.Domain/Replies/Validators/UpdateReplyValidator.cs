using Atlas.Domain.Replies.Commands;
using FluentValidation;

namespace Atlas.Domain.Replies.Validators
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
