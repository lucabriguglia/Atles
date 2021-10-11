using System;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain.Categories;
using Atles.Domain.Forums;
using Atles.Domain.Handlers.Posts.Commands;
using Atles.Domain.Posts;
using Atles.Domain.Posts.Commands;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Domain.Handlers.Tests.Posts.Commands
{
    [TestFixture]
    public class PinTopicHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_pin_topic_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var siteId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var forumId = Guid.NewGuid();

            var category = new Category(categoryId, siteId, "Category", 1, Guid.NewGuid());
            var forum = new Forum(forumId, category.Id, "Forum", "my-forum", "My Forum", 1);
            var topic = Post.CreateTopic(forumId, Guid.NewGuid(), "Title", "slug", "Content", PostStatusType.Published);

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                dbContext.Posts.Add(topic);

                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var command = Fixture.Build<PinTopic>()
                    .With(x => x.Id, topic.Id)
                    .With(x => x.ForumId, forumId)
                    .With(x => x.SiteId, siteId)
                    .Create();

                var cacheManager = new Mock<ICacheManager>();

                var sut = new PinTopicHandler(dbContext, cacheManager.Object);

                await sut.Handle(command);

                var pinnedTopic = await dbContext.Posts.FirstOrDefaultAsync(x => x.Id == command.Id);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                Assert.AreEqual(command.Pinned, pinnedTopic.Pinned);
                Assert.NotNull(@event);
            }
        }
    }
}
