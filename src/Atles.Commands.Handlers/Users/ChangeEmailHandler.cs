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

public class ChangeEmailHandler : ICommandHandler<ChangeEmail>
{
    private readonly AtlesDbContext _dbContext;

    public ChangeEmailHandler(AtlesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CommandResult> Handle(ChangeEmail command)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.IdentityUserId == command.IdentityUserId);

        if (user == null)
        {
            throw new DataException($"User with IdentityUserId {command.IdentityUserId} not found.");
        }

        user.UpdateEmail(command.Email);

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
