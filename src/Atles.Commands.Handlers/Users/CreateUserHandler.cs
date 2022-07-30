using Atles.Commands.Users;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Core.Results;
using Atles.Data;
using Atles.Domain;
using Atles.Events.Users;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Atles.Commands.Handlers.Users
{
    public class CreateUserHandler : ICommandHandler<CreateUser>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly IValidator<CreateUser> _validator;

        public CreateUserHandler(AtlesDbContext dbContext, IValidator<CreateUser> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public async Task<CommandResult> Handle(CreateUser command)
        {
            await _validator.ValidateCommand(command);

            var displayName = await GenerateDisplayNameAsync();

            var user = new User(command.CreateUserId,
                command.IdentityUserId,
                command.Email,
                displayName);

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

            return new Success(new IEvent[] { @event });
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
}
