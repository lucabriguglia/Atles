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

public class UpdateUserHandler : ICommandHandler<UpdateUser>
{
    private readonly AtlesDbContext _dbContext;
    private readonly UserManager<IdentityUser> _userManager;

    public UpdateUserHandler(AtlesDbContext dbContext, UserManager<IdentityUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<CommandResult> Handle(UpdateUser command)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x =>
                x.Id == command.Id);

        if (user == null)
        {
            return new Failure(FailureType.NotFound, "User", $"User with Id {command.Id} not found.");
        }

        var identityUser = await _userManager.FindByIdAsync(command.IdentityUserId);
        if (identityUser is not null && command.Roles.Any())
        {
            await ProcessRoles(command, identityUser);
        }

        user.UpdateDetails(command.DisplayName);

        var selectedRoles = command.Roles.Where(role => role.Selected).ToList() is {Count: > 0}
            ? string.Join(", ", command.Roles)
            : string.Empty;

        var @event = new UserUpdated
        {
            DisplayName = user.DisplayName,
            Roles = selectedRoles,
            TargetId = user.Id,
            TargetType = nameof(User),
            SiteId = command.SiteId,
            UserId = command.UserId
        };

        _dbContext.Events.Add(@event.ToDbEntity());

        await _dbContext.SaveChangesAsync();

        return new Success(new IEvent[] { @event });
    }

    private async Task ProcessRoles(UpdateUser command, IdentityUser identityUser)
    {
        foreach (var (name, selected) in command.Roles)
        {
            if (selected)
            {
                if (!await _userManager.IsInRoleAsync(identityUser, name))
                {
                    await _userManager.AddToRoleAsync(identityUser, name);
                }
            }
            else
            {
                if (await _userManager.IsInRoleAsync(identityUser, name))
                {
                    await _userManager.RemoveFromRoleAsync(identityUser, name);
                }
            }
        }
    }
}
