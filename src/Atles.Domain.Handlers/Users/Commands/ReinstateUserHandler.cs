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
    public class ReinstateUserHandler : ICommandHandler<ReinstateUser>
    {
        private readonly AtlesDbContext _dbContext;

        public ReinstateUserHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<IEvent>> Handle(ReinstateUser command)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.Status != UserStatusType.Deleted);

            if (user == null)
            {
                throw new DataException($"User with Id {command.Id} not found.");
            }

            user.Reinstate();

            var @event = new UserReinstated
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
