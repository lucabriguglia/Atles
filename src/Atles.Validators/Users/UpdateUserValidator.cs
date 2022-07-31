using Atles.Commands.Users;
using Atles.Core;
using Atles.Domain.Rules.Users;
using FluentValidation;

namespace Atles.Validators.Users
{
    public class UpdateUserValidator : AbstractValidator<UpdateUser>
    {
        public UpdateUserValidator(IDispatcher dispatcher)
        {
            RuleFor(c => c.DisplayName)
                .NotEmpty().WithMessage("Display name is required.")
                .Length(1, 50).WithMessage("Display name must be at least 1 and at max 50 characters long.")
                .MustAsync(async (c, p, cancellation) =>
                {
                    // TODO: To be moved to a service
                    var result = await dispatcher.Get(new IsUserDisplayNameUnique { DisplayName = p, Id = c.UpdateUserId });
                    return result.AsT0;
                })
                .WithMessage(c => $"A user with display name {c.DisplayName} already exists.");
        }
    }
}
