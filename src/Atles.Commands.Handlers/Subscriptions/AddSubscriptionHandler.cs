using Atles.Commands.Subscriptions;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Core.Results;
using Atles.Core.Results.Types;
using Atles.Data;
using Atles.Domain;
using Atles.Events;
using FluentValidation;

namespace Atles.Commands.Handlers.Subscriptions
{
    public class AddSubscriptionHandler : ICommandHandler<AddSubscription>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly IValidator<AddSubscription> _validator;

        public AddSubscriptionHandler(AtlesDbContext dbContext, IValidator<AddSubscription> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public async Task<CommandResult> Handle(AddSubscription command)
        {
            await _validator.ValidateAsync(command);

            var subscription = new Subscription(command.UserId, command.Type, command.ItemId);

            _dbContext.Subscriptions.Add(subscription);

            var @event = new SubscriptionAdded
            {
                Type = command.Type,
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
