using System.Data;
using Atles.Commands.Users;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Data;
using Atles.Domain.Events.Users;
using Atles.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Atles.Domain.Commands.Handlers.Users
{
    public class SuspendUserHandler : ICommandHandler<SuspendUser>
    {
        private readonly AtlesDbContext _dbContext;

        public SuspendUserHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<IEvent>> Handle(SuspendUser command)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == command.SuspendUserId &&
                    x.Status != UserStatusType.Deleted);

            if (user == null)
            {
                throw new DataException($"User with Id {command.SuspendUserId} not found.");
            }

            user.Suspend();

            var @event = new UserSuspended
            {
                TargetId = user.Id,
                TargetType = nameof(User),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(@event.ToDbEntity());

            await _dbContext.SaveChangesAsync();

            return new IEvent[] { @event };
        }
    }
}
