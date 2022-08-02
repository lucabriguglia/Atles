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
    public class ChangeEmailHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_update_user_and_add_event()
        {
            var options = Shared.CreateContextOptions();
            var user = new User(Guid.NewGuid(), Guid.NewGuid().ToString(), "me@email.com", "Display Name");

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var command = Fixture.Build<ChangeEmail>()
                        .With(x => x.IdentityUserId, user.IdentityUserId)
                        .Create();

                var sut = new ChangeEmailHandler(dbContext);

                await sut.Handle(command);

                var updatedUser = await dbContext.Users.FirstOrDefaultAsync(x => x.IdentityUserId == command.IdentityUserId);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == updatedUser.Id);

                Assert.AreEqual(command.Email, updatedUser.Email);
                Assert.NotNull(@event);
            }
        }
    }
}
