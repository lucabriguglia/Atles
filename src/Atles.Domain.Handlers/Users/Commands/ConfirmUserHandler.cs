using Atles.Data;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Commands;
using System;
using System.Data;
using System.Threading.Tasks;
using Atles.Domain.Models;
using Atles.Domain.Models.Users;
using Atles.Domain.Models.Users.Commands;

namespace Atles.Domain.Handlers.Users.Commands
{
    public class ConfirmUserHandler : ICommandHandler<ConfirmUser>
    {
        private readonly AtlesDbContext _dbContext;

        public ConfirmUserHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(ConfirmUser command)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.Status == UserStatusType.Pending);

            if (user == null)
            {
                throw new DataException($"User with Id {command.Id} not found.");
            }

            user.Confirm();

            var userIdForEvent = command.UserId == Guid.Empty
                ? user.Id
                : command.UserId;

            _dbContext.Events.Add(new Event(command.SiteId,
                userIdForEvent,
                EventType.Confirmed,
                typeof(User),
                command.Id));

            await _dbContext.SaveChangesAsync();
        }
    }
}
