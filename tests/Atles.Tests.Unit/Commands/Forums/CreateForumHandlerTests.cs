using Atles.Commands.Forums;
using Atles.Commands.Handlers.Forums;
using Atles.Data;
using Atles.Data.Caching;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Commands.Forums;

[TestFixture]
public class CreateForumHandlerTests : TestFixtureBase
{
    [Test]
    public async Task Should_create_new_forum_and_add_event()
    {
        await using var dbContext = new AtlesDbContext(Shared.CreateContextOptions());

        var command = Fixture.Create<CreateForum>();

        var cacheManager = new Mock<ICacheManager>();

        var sut = new CreateForumHandler(dbContext, cacheManager.Object);

        await sut.Handle(command);

        var forum = await dbContext.Forums.FirstOrDefaultAsync(x => x.Id == command.ForumId);
        var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.ForumId);

        Assert.NotNull(forum);
        Assert.AreEqual(1, forum.SortOrder);
        Assert.NotNull(@event);
    }
}
