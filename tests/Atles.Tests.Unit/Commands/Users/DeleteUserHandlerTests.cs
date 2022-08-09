using Atles.Commands.Handlers.Users;
using Atles.Commands.Users;
using Atles.Data;
using Atles.Domain;
using AutoFixture;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Commands.Users
{
    [Ignore("WIP")]
    [TestFixture]
    public class DeleteUserHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_delete_user_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var userId = Guid.NewGuid();
            var identityUserId = Guid.NewGuid();

            var user = new User(userId, identityUserId.ToString(), "me@email.com", "Display Name");
            var subscription = new Subscription(userId, SubscriptionType.Forum, Guid.NewGuid());

            await using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Users.Add(user);
                dbContext.Subscriptions.Add(subscription);
                await dbContext.SaveChangesAsync();
            }

            await using (var dbContext = new AtlesDbContext(options))
            {
                var command = Fixture.Build<DeleteUser>()
                        .With(x => x.DeleteUserId, user.Id)
                        .With(x => x.IdentityUserId, user.IdentityUserId)
                        .Create();

                var userManager = new Mock<UserManager<IdentityUser>>();
                // TODO: Setup user manager

                var sut = new DeleteUserHandler(dbContext, userManager.Object);

                await sut.Handle(command);

                var userDeleted = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
                var subscriptions = await dbContext.Subscriptions.Where(x => x.UserId == userId).ToListAsync();
                var userEvent = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == user.Id);

                Assert.AreEqual(UserStatusType.Deleted, userDeleted.Status);
                Assert.Zero(subscriptions.Count);
                Assert.NotNull(userEvent);
            }
        }
    }
}
