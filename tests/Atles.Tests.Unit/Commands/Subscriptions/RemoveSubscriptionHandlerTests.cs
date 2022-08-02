using System.Data;
using Atles.Commands.Handlers.Subscriptions;
using Atles.Commands.Subscriptions;
using Atles.Data;
using Atles.Domain;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Atles.Tests.Unit.Commands.Subscriptions
{
    [TestFixture]
    public class RemoveSubscriptionHandlerTests : TestFixtureBase
    {
        [Test]
        public void Should_throw_data_exception_when_subscription_not_found()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var sut = new RemoveSubscriptionHandler(dbContext);
                Assert.ThrowsAsync<DataException>(async () => await sut.Handle(Fixture.Create<RemoveSubscription>()));
            }
        }

        [Test]
        public async Task Should_remove_subscription()
        {
            var options = Shared.CreateContextOptions();

            var subscription = new Subscription(Guid.NewGuid(), SubscriptionType.Topic, Guid.NewGuid());

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Subscriptions.Add(subscription);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var command = Fixture.Build<RemoveSubscription>()
                    .With(x => x.ItemId, subscription.ItemId)
                    .With(x => x.UserId, subscription.UserId)
                    .Create();

                var sut = new RemoveSubscriptionHandler(dbContext);

                await sut.Handle(command);

                var deletedSubscription = await dbContext.Subscriptions.FirstOrDefaultAsync(x => x.UserId == command.UserId && x.ItemId == command.ItemId);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.UserId);

                Assert.Null(deletedSubscription);
                Assert.NotNull(@event);
            }
        }
    }
}
