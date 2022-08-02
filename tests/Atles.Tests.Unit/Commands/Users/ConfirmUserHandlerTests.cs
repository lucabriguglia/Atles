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
    public class ConfirmUserHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_confirm_user_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var user = new User(Guid.NewGuid().ToString(), "me@email.com", "Display Name");

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var command = Fixture.Build<ConfirmUser>()
                    .With(x => x.IdentityUserId, user.IdentityUserId)
                    .Create();

                var sut = new ConfirmUserHandler(dbContext);

                await sut.Handle(command);

                var userConfirmed = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
                var userEvent = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == user.Id);

                Assert.AreEqual(UserStatusType.Active, userConfirmed.Status);
                Assert.NotNull(userEvent);
            }
        }
    }
}
