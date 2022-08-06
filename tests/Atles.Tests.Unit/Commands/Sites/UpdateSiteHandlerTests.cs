using Atles.Commands.Handlers.Sites;
using Atles.Commands.Sites;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Commands.Sites;

[TestFixture]
public class UpdateSiteHandlerTests : TestFixtureBase
{
    [Test]
    public async Task Should_update_site_and_add_event()
    {
        var options = Shared.CreateContextOptions();
        var site = new Site(Guid.NewGuid(), "Name", "Title");

        await using (var dbContext = new AtlesDbContext(options))
        {
            dbContext.Sites.Add(site);
            await dbContext.SaveChangesAsync();
        }

        await using (var dbContext = new AtlesDbContext(options))
        {
            var command = Fixture.Build<UpdateSite>()
                .With(x => x.SiteId, site.Id)
                .Create();

            var cacheManager = new Mock<ICacheManager>();

            var sut = new UpdateSiteHandler(dbContext, cacheManager.Object);

            await sut.Handle(command);

            var updatedSite = await dbContext.Sites.FirstOrDefaultAsync(x => x.Id == command.SiteId);
            var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.SiteId);

            Assert.AreEqual(command.Title, updatedSite.Title);
            Assert.NotNull(@event);
        }
    }
}
