using Atles.Data;
using Atles.Domain.Users;
using Atles.Domain.Users.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Commands;
using System;
using System.Threading.Tasks;

namespace Atles.Domain.Handlers.Users.Commands
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

        public async Task Handle(CreateUser command)
        {
            await _validator.ValidateCommandAsync(command);

            var displayName = await GenerateDisplayNameAsync();

            var user = new User(command.Id,
                command.IdentityUserId,
                command.Email,
                displayName);

            if (command.Confirm)
            {
                user.Confirm();
            }

            _dbContext.Users.Add(user);

            var userIdForEvent = command.UserId == Guid.Empty
                ? user.Id
                : command.UserId;

            _dbContext.Events.Add(new Event(command.SiteId,
                userIdForEvent,
                EventType.Created,
                typeof(User),
                command.Id,
                new
                {
                    UserId = command.IdentityUserId,
                    command.Email,
                    DisplayName = displayName,
                    user.Status
                }));

            await _dbContext.SaveChangesAsync();
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
