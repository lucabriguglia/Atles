using System.Data;
using Atles.Commands.Users;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Data;
using Atles.Domain.Events.Users;
using Atles.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Atles.Commands.Handlers.Users
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
                    x.Id == command.ReinstateUserId &&
                    x.Status != UserStatusType.Deleted);

            if (user == null)
            {
                throw new DataException($"User with Id {command.ReinstateUserId} not found.");
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
