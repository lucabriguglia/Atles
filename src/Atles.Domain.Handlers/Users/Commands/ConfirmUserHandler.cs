using Atles.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Threading.Tasks;
using Atles.Data.Configurations;
using Atles.Domain.Models;
using Atles.Domain.Models.Users;
using Atles.Domain.Models.Users.Commands;
using Atles.Domain.Models.Users.Events;
using Atles.Infrastructure.Commands;

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

            var @event = new UserConfirmed
            {
                TargetId = user.Id,
                TargetType = nameof(User),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(@event.ToDbEntity());

            await _dbContext.SaveChangesAsync();
        }
    }
}
