using Atles.Commands.Forums;
using Atles.Commands.Handlers.Forums;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Commands.Forums
{
    [TestFixture]
    public class DeleteForumHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_delete_forum_and_reorder_other_forums_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var categoryId = Guid.NewGuid();
            var siteId = Guid.NewGuid();

            var category = new Category(categoryId, siteId, "Category", 1, Guid.NewGuid());

            var forum1 = new Forum(categoryId, "Forum 1", "my-forum-1", "Forum 1", 1);
            var forum2 = new Forum(categoryId, "Forum 2", "my-forum-2", "Forum 2", 2);
            var forum3 = new Forum(categoryId, "Forum 3", "my-forum-3", "Forum 3", 3);
            var forum4 = new Forum(categoryId, "Forum 4", "my-forum-4", "Forum 4", 4);

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum1);
                dbContext.Forums.Add(forum2);
                dbContext.Forums.Add(forum3);
                dbContext.Forums.Add(forum4);

                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var command = Fixture.Build<DeleteForum>()
                        .With(x => x.SiteId, siteId)
                        .With(x => x.ForumId, forum2.Id)
                    .Create();

                var cacheManager = new Mock<ICacheManager>();

                var sut = new DeleteForumHandler(dbContext,
                    cacheManager.Object);

                await sut.Handle(command);

                var forum1Reordered = await dbContext.Forums.FirstOrDefaultAsync(x => x.Id == forum1.Id);
                var forum2Deleted = await dbContext.Forums.FirstOrDefaultAsync(x => x.Id == forum2.Id);
                var forum3Reordered = await dbContext.Forums.FirstOrDefaultAsync(x => x.Id == forum3.Id);
                var forum4Reordered = await dbContext.Forums.FirstOrDefaultAsync(x => x.Id == forum4.Id);

                var forum1Event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == forum1.Id);
                var forum2Event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == forum2.Id);
                var forum3Event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == forum3.Id);
                var forum4Event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == forum4.Id);

                Assert.AreEqual(forum1.SortOrder, forum1Reordered.SortOrder);
                Assert.AreEqual(ForumStatusType.Deleted, forum2Deleted.Status);
                Assert.AreEqual(forum2.SortOrder, forum3Reordered.SortOrder);
                Assert.AreEqual(forum3.SortOrder, forum4Reordered.SortOrder);
                Assert.NotNull(forum1Event);
                Assert.NotNull(forum2Event);
                Assert.NotNull(forum3Event);
                Assert.NotNull(forum4Event);
            }
        }
    }
}
