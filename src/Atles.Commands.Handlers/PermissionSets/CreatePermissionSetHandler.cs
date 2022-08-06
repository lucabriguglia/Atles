using Atles.Commands.PermissionSets;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Core.Results;
using Atles.Core.Results.Types;
using Atles.Data;
using Atles.Domain;
using Atles.Events.PermissionSets;

namespace Atles.Commands.Handlers.PermissionSets;

public class CreatePermissionSetHandler : ICommandHandler<CreatePermissionSet>
{
    private readonly AtlesDbContext _dbContext;

    public CreatePermissionSetHandler(AtlesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CommandResult> Handle(CreatePermissionSet command)
    {
        var permissionSet = new PermissionSet(command.PermissionSetId,
            command.SiteId,
            command.Name,
            command.Permissions.ToDomainPermissions());

        _dbContext.PermissionSets.Add(permissionSet);

        var @event = new PermissionSetCreated
        {
            Name = permissionSet.Name,
            Permissions = command.Permissions.ToDomainPermissions(),
            TargetId = permissionSet.Id,
            TargetType = nameof(PermissionSet),
            SiteId = command.SiteId,
            UserId = command.UserId
        };

        _dbContext.Events.Add(@event.ToDbEntity());

        await _dbContext.SaveChangesAsync();

        return new Success(new IEvent[] { @event });
    }
}
