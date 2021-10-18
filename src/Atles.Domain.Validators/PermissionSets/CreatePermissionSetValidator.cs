using Atles.Domain.PermissionSets.Commands;
using Atles.Domain.PermissionSets.Rules;
using FluentValidation;
using OpenCqrs;

namespace Atles.Domain.Validators.PermissionSets
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
