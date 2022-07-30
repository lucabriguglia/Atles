using Atles.Commands.PermissionSets;
using Atles.Core;
using Atles.Domain.Rules.PermissionSets;
using FluentValidation;

namespace Atles.Domain.Commands.Handlers.PermissionSets.Validators
{
    public class CreatePermissionSetValidator : AbstractValidator<CreatePermissionSet>
    {
        public CreatePermissionSetValidator(IDispatcher dispatcher)
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Permission set name is required.")
                .Length(1, 50).WithMessage("Permission set name must be at least 1 and at max 50 characters long.")
                .MustAsync((c, p, cancellation) => dispatcher.Get(new IsPermissionSetNameUnique { SiteId = c.SiteId, Name = p }))
                    .WithMessage(c => $"A permission set with name {c.Name} already exists.");
        }
    }
}
