using Atles.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Atles.Domain.Models;
using Atles.Domain.Models.Users;
using Atles.Domain.Models.Users.Commands;
using Atles.Domain.Models.Users.Events;
using Atles.Infrastructure.Commands;

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
            await _validator.ValidateCommand(command);

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

            var @event = new UserCreated
            {
                IdentityUserId = user.IdentityUserId,
                Email = user.Email,
                DisplayName = displayName,
                Status = user.Status,
                TargetId = command.UserId == Guid.Empty ? user.Id : command.UserId,
                TargetType = nameof(User),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(@event.ToDbEntity());

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
