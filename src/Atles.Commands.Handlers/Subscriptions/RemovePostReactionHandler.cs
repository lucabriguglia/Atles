using System.Data;
using Atles.Commands.Subscriptions;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Core.Results;
using Atles.Core.Results.Types;
using Atles.Data;
using Atles.Domain;
using Atles.Events;
using Microsoft.EntityFrameworkCore;

namespace Atles.Commands.Handlers.Subscriptions
{
    public class RemoveSubscriptionHandler : ICommandHandler<RemoveSubscription>
    {
        private readonly AtlesDbContext _dbContext;

        public RemoveSubscriptionHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CommandResult> Handle(RemoveSubscription command)
        {
            var subscription = await _dbContext.Subscriptions
                .FirstOrDefaultAsync(x =>
                    x.ItemId == command.ItemId &&
                    x.UserId == command.UserId);

            if (subscription == null)
            {
                throw new DataException($"Subscription for item id {command.ItemId} and user id {command.UserId} not found.");
            }

            _dbContext.Subscriptions.Remove(subscription);

            var @event = new SubscriptionRemoved
            {
                Type = subscription.Type,
                ItemId = command.ItemId,
                TargetId = command.UserId,
                TargetType = nameof(User),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(@event.ToDbEntity());

            await _dbContext.SaveChangesAsync();

            return new Success(new IEvent[] { @event });
        }
    }
}
