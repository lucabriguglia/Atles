using Atles.Commands.Handlers.Users;
using Atles.Commands.Users;
using Atles.Data;
using AutoFixture;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Commands.Users;

[Ignore("WIP")]
[TestFixture]
public class CreateUserHandlerTests : TestFixtureBase
{
    [Test]
    public async Task Should_create_new_user_and_add_event()
    {
        await using var dbContext = new AtlesDbContext(Shared.CreateContextOptions());
        
        var command = Fixture.Create<CreateUser>();

        var userManager = new Mock<UserManager<IdentityUser>>();
        // TODO: Setup user manager

        var sut = new CreateUserHandler(dbContext, userManager.Object);

        await sut.Handle(command);

        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.IdentityUserId == command.IdentityUserId);
        var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == user.Id);

        Assert.NotNull(user);
        Assert.NotNull(@event);
    }
}
