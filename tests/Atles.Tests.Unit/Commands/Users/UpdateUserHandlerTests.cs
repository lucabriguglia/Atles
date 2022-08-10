using Atles.Commands.Handlers.Users;
using Atles.Commands.Users;
using Atles.Data;
using Atles.Domain;
using AutoFixture;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Commands.Users;

[Ignore("WIP")]
[TestFixture]
public class UpdateUserHandlerTests : TestFixtureBase
{
    [Test]
    public async Task Should_update_user_and_add_event()
    {
        var options = Shared.CreateContextOptions();
        var user = new User(Guid.NewGuid(), Guid.NewGuid().ToString(), "me@email.com", "Display Name");

        await using (var dbContext = new AtlesDbContext(options))
        {
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
        }

        await using (var dbContext = new AtlesDbContext(options))
        {
            var command = Fixture.Build<UpdateUser>()
                .With(x => x.Id, user.Id)
                .Create();

            var userManager = new Mock<UserManager<IdentityUser>>();
            // TODO: Setup user manager

            var sut = new UpdateUserHandler(dbContext, userManager.Object);

            await sut.Handle(command);

            var updatedUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == command.Id);
            var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);

            Assert.AreEqual(command.DisplayName, updatedUser.DisplayName);
            Assert.NotNull(@event);
        }
    }
}
