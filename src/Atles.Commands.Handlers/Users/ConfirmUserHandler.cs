using System.Data;
using Atles.Commands.Users;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Core.Results;
using Atles.Core.Results.Types;
using Atles.Data;
using Atles.Domain;
using Atles.Events.Users;
using Microsoft.EntityFrameworkCore;

namespace Atles.Commands.Handlers.Users;

public class ConfirmUserHandler : ICommandHandler<ConfirmUser>
{
    private readonly AtlesDbContext _dbContext;

    public ConfirmUserHandler(AtlesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CommandResult> Handle(ConfirmUser command)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x =>
                x.IdentityUserId == command.IdentityUserId &&
                x.Status == UserStatusType.Pending);

        if (user == null)
        {
            throw new DataException($"User with Id {command.IdentityUserId} not found.");
        }

        user.Confirm();

        var @event = new UserConfirmed
        {
            TargetId = user.Id,
            TargetType = nameof(User),
            SiteId = command.SiteId,
            UserId = user.Id
        };

        _dbContext.Events.Add(@event.ToDbEntity());

        await _dbContext.SaveChangesAsync();

        return new Success(new IEvent[] { @event });
    }
}
