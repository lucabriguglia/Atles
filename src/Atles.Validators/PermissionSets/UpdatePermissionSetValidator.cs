using Atles.Commands.PermissionSets;
using Atles.Core;
using Atles.Domain.Rules.PermissionSets;
using FluentValidation;

namespace Atles.Validators.PermissionSets
{
    public class UpdatePermissionSetValidator : AbstractValidator<UpdatePermissionSet>
    {
        public UpdatePermissionSetValidator(IDispatcher dispatcher)
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Permission set name is required.")
                .Length(1, 50).WithMessage("Permission set name must be at least 1 and at max 50 characters long.")
                .MustAsync(async (c, p, cancellation) =>
                {
                    // TODO: To be moved to a service
                    var result = await dispatcher.Get(new IsPermissionSetNameUnique
                            {SiteId = c.SiteId, Name = p, Id = c.PermissionSetId});
                    return result.AsT0;
                })
                    .WithMessage(c => $"A permission set with name {c.Name} already exists.");
        }
    }
}
