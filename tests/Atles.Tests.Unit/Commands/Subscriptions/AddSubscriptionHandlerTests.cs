using Atles.Commands.Handlers.Subscriptions;
using Atles.Commands.Subscriptions;
using Atles.Data;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Commands.Subscriptions
{
    [TestFixture]
    public class AddSubscriptionHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_add_subscription()
        {
            await using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var command = Fixture.Create<AddSubscription>();

                var validator = new Mock<IValidator<AddSubscription>>();
                validator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var sut = new AddSubscriptionHandler(dbContext, validator.Object);

                await sut.Handle(command);

                var subscription = await dbContext.Subscriptions.FirstOrDefaultAsync(x => x.UserId == command.UserId && x.ItemId == command.ItemId);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.UserId);

                validator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.NotNull(subscription);
                Assert.NotNull(@event);
            }
        }
    }
}
