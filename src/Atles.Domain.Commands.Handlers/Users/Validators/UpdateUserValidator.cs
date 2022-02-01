using Atles.Core;
using Atles.Domain.Commands.Users;
using Atles.Domain.Rules;
using Atles.Domain.Rules.Users;
using FluentValidation;

namespace Atles.Domain.Commands.Handlers.Users.Validators
{
    public class UpdateUserValidator : AbstractValidator<UpdateUser>
    {
        public UpdateUserValidator(IDispatcher dispatcher)
        {
            RuleFor(c => c.DisplayName)
                .NotEmpty().WithMessage("Display name is required.")
                .Length(1, 50).WithMessage("Display name must be at least 1 and at max 50 characters long.")
                .MustAsync((c, p, cancellation) => dispatcher.Get(new IsUserDisplayNameUnique { DisplayName = p, Id = c.UpdateUserId }))
                    .WithMessage(c => $"A user with display name {c.DisplayName} already exists.");
        }
    }
}
