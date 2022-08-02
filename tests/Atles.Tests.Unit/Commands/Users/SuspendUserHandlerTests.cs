using Atles.Commands.Handlers.Users;
using Atles.Commands.Users;
using Atles.Data;
using Atles.Domain;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Atles.Tests.Unit.Commands.Users
{
    [TestFixture]
    public class SuspendUserHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_suspend_user_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var userId = Guid.NewGuid();

            var user = new User(userId, Guid.NewGuid().ToString(), "me@email.com", "Display Name");

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var command = Fixture.Build<SuspendUser>()
                    .With(x => x.SuspendUserId, user.Id)
                    .Create();

                var sut = new SuspendUserHandler(dbContext);

                await sut.Handle(command);

                var userSuspended = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
                var userEvent = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == user.Id);

                Assert.AreEqual(UserStatusType.Suspended, userSuspended.Status);
                Assert.NotNull(userEvent);
            }
        }
    }
}
