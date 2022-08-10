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

public class CreateUserHandler : ICommandHandler<CreateUser>
{
    private readonly AtlesDbContext _dbContext;
    private readonly UserManager<IdentityUser> _userManager;

    public CreateUserHandler(AtlesDbContext dbContext, UserManager<IdentityUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<CommandResult> Handle(CreateUser command)
    {
        var identityUserId = command.IdentityUserId;

        if (string.IsNullOrEmpty(identityUserId))
        {
            var identityUser = new IdentityUser { UserName = command.Email, Email = command.Email };
            identityUserId = identityUser.Id;

            var createResult = await _userManager.CreateAsync(identityUser, command.Password);

            if (!createResult.Succeeded)
            {
                return new Failure(FailureType.Error, "Error creating identity user", createResult.ToString());
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
            var confirmResult = await _userManager.ConfirmEmailAsync(identityUser, token);
            // TODO: Handle confirmResult not succeeded
        }

        var displayName = await GenerateDisplayNameAsync();

        var user = new User(identityUserId, command.Email, displayName);

        if (command.Confirm)
        {
            user.Confirm();
        }

        _dbContext.Users.Add(user);

        var @event = new UserCreated
        {
            IdentityUserId = user.IdentityUserId,
            Email = user.Email,
            DisplayName = displayName,
            Status = user.Status,
            TargetId = user.Id,
            TargetType = nameof(User),
            SiteId = command.SiteId,
            UserId = command.UserId
        };

        _dbContext.Events.Add(@event.ToDbEntity());

        await _dbContext.SaveChangesAsync();

        return new Success(new IEvent[] { @event }, user.Id);
    }

    private async Task<string> GenerateDisplayNameAsync()
    {
        var displayName = string.Empty;
        var exists = true;
        var repeat = 0;
        var random = new Random();

        while (exists && repeat < 5)
        {
            displayName = $"User{random.Next(100000)}";
            exists = await _dbContext.Users.AnyAsync(x => x.DisplayName == displayName);
            repeat++;
        }

        if (exists)
        {
            displayName = Guid.NewGuid().ToString();
        }

        return displayName;
    }
}
