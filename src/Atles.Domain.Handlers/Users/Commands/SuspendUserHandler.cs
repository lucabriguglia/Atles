using System.Collections.Generic;
using Atles.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading.Tasks;
using Atles.Domain.Commands;
using Atles.Domain.Events;
using Atles.Domain.Models;
using Atles.Infrastructure.Commands;
using Atles.Infrastructure.Events;

namespace Atles.Domain.Handlers.Users.Commands
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
                    x.Id == command.Id &&
                    x.Status != UserStatusType.Deleted);

            if (user == null)
            {
                throw new DataException($"User with Id {command.Id} not found.");
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
