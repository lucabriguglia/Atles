using Atlas.Domain.Posts.Commands;
using FluentValidation;

namespace Atlas.Domain.Posts.Validators
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
