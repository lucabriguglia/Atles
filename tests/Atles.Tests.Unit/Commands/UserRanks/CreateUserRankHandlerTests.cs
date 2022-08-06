using Atles.Commands.Handlers.UserRanks;
using Atles.Commands.UserRanks;
using Atles.Data;
using Atles.Data.Caching;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Commands.UserRanks;

[TestFixture]
public class CreateUserRankHandlerTests : TestFixtureBase
{
    [Test]
    public async Task Should_create_new_userRank_and_add_event()
    {
        await using var dbContext = new AtlesDbContext(Shared.CreateContextOptions());
            
        var command = Fixture.Create<CreateUserRank>();

        var cacheManager = new Mock<ICacheManager>();

        var sut = new CreateUserRankHandler(dbContext, cacheManager.Object);

        await sut.Handle(command);

        var userRank = await dbContext.UserRanks.FirstOrDefaultAsync();
        var @event = await dbContext.Events.FirstOrDefaultAsync();

        Assert.NotNull(userRank);
        Assert.AreEqual(1, userRank.SortOrder);
        Assert.NotNull(@event);
    }
}
