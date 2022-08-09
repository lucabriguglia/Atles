using System.Data;
using Atles.Commands.Users;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Core.Results;
using Atles.Core.Results.Types;
using Atles.Data;
using Atles.Domain;
using Atles.Events.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atles.Commands.Handlers.Users;

public class DeleteUserHandler : ICommandHandler<DeleteUser>
{
    private readonly AtlesDbContext _dbContext;
    private readonly UserManager<IdentityUser> _userManager;

    public DeleteUserHandler(AtlesDbContext dbContext, UserManager<IdentityUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<CommandResult> Handle(DeleteUser command)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x =>
                x.Id == command.DeleteUserId &&
                x.IdentityUserId == command.IdentityUserId &&
                x.Status != UserStatusType.Deleted);

        if (user == null)
        {
            return new Failure(FailureType.NotFound, "User", $"User with Id {command.DeleteUserId} not found.");
        }

        user.Delete();

        var existingSubscriptions = _dbContext.Subscriptions.Where(x => x.UserId == user.Id);

        foreach (var subscription in existingSubscriptions)
        {
            _dbContext.Subscriptions.Remove(subscription);
        }

        var @event = new UserDeleted
        {
            RemovedSubscriptions = existingSubscriptions.Count(),
            TargetId = user.Id,
            TargetType = nameof(User),
            SiteId = command.SiteId,
            UserId = command.UserId
        };

        _dbContext.Events.Add(@event.ToDbEntity());

        await _dbContext.SaveChangesAsync();

        var identityUser = await _userManager.FindByIdAsync(command.IdentityUserId);
        if (identityUser != null)
        {
            await _userManager.DeleteAsync(identityUser);
        }

        return new Success(new IEvent[] { @event });
    }
}
