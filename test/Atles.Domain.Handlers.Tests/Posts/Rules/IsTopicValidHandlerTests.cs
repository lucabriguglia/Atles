using System;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain.Handlers.Posts.Rules;
using Atles.Domain.Models;
using Atles.Domain.Rules;
using NUnit.Framework;

namespace Atles.Domain.Handlers.Tests.Posts.Rules
{
    [TestFixture]
    public class IsTopicValidHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_return_true_when_topic_is_valid()
        {
            var options = Shared.CreateContextOptions();
            var category = new Category(Guid.NewGuid(), Guid.NewGuid(), "Category", 1, Guid.NewGuid());
            var forum = new Forum(Guid.NewGuid(), category.Id, "Forum", "my-forum", "My Forum", 1, Guid.NewGuid());
            var topic = Post.CreateTopic(Guid.NewGuid(), forum.Id, Guid.NewGuid(), "Title", "slug", "Content", PostStatusType.Published);

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                dbContext.Posts.Add(topic);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var sut = new IsTopicValidHandler(dbContext);
                var query = new IsTopicValid { SiteId = category.SiteId, ForumId = forum.Id, Id = topic.Id };
                var actual = await sut.Handle(query);

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_false_when_topic_is_not_valid()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var sut = new IsTopicValidHandler(dbContext);
                var query = new IsTopicValid { SiteId = Guid.NewGuid(), ForumId = Guid.NewGuid(), Id = Guid.NewGuid() };
                var actual = await sut.Handle(query);

                Assert.IsFalse(actual);
            }
        }
    }
}
