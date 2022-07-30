using System.Data;
using Atles.Commands.Users;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Data;
using Atles.Domain;
using Atles.Domain.Events.Users;
using Microsoft.EntityFrameworkCore;

namespace Atles.Commands.Handlers.Users
{
    public class DeleteUserHandler : ICommandHandler<DeleteUser>
    {
        private readonly AtlesDbContext _dbContext;

        public DeleteUserHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<IEvent>> Handle(DeleteUser command)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == command.DeleteUserId &&
                    x.IdentityUserId == command.IdentityUserId &&
                    x.Status != UserStatusType.Deleted);

            if (user == null)
            {
                throw new DataException($"User with Id {command.DeleteUserId} not found.");
            }

            user.Delete();

            var existingSubscriptions = _dbContext.Subscriptions.Where(x => x.UserId == user.Id);

            foreach (var subscription in existingSubscriptions)
            {
                _dbContext.Subscriptions.Remove(subscription);
            }

            var @event = new UserDeleted
            {
                RemovedSubscriptions = existingSubscriptions.Count(),
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
