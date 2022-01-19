using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain.Models;
using Atles.Domain.Models.PermissionSets;
using Atles.Domain.Models.PermissionSets.Commands;
using Atles.Infrastructure.Commands;
using FluentValidation;

namespace Atles.Domain.Handlers.PermissionSets.Commands
{
    public class CreatePermissionSetHandler : ICommandHandler<CreatePermissionSet>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly IValidator<CreatePermissionSet> _validator;

        public CreatePermissionSetHandler(AtlesDbContext dbContext,
            IValidator<CreatePermissionSet> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public async Task Handle(CreatePermissionSet command)
        {
            await _validator.ValidateCommandAsync(command);

            var permissionSet = new PermissionSet(command.Id,
                command.SiteId,
                command.Name,
                command.Permissions);

            _dbContext.PermissionSets.Add(permissionSet);

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Created,
                typeof(PermissionSet),
                command.Id,
                new
                {
                    command.SiteId,
                    command.Name,
                    command.Permissions
                }));

            await _dbContext.SaveChangesAsync();
        }
    }
}
