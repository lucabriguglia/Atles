using Atles.Domain.Models.Posts.Commands;
using FluentValidation;

namespace Atles.Domain.Validators.Posts
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
