using Atles.Domain.PermissionSets.Commands;
using Atles.Domain.PermissionSets.Rules;
using FluentValidation;
using OpenCqrs.Queries;

namespace Atles.Domain.PermissionSets.Validators
{
    public class CreatePermissionSetValidator : AbstractValidator<CreatePermissionSet>
    {
        public CreatePermissionSetValidator(IQuerySender queries)
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Permission set name is required.")
                .Length(1, 50).WithMessage("Permission set name must be at least 1 and at max 50 characters long.")
                .MustAsync((c, p, cancellation) => queries.Send(new IsPermissionSetNameUnique { SiteId = c.SiteId, Name = p }))
                    .WithMessage(c => $"A permission set with name {c.Name} already exists.");
        }
    }
}
