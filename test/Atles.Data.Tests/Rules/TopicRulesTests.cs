using System;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Data.Rules;
using Atles.Domain.Categories;
using Atles.Domain.Forums;
using Atles.Domain.Posts;
using NUnit.Framework;

namespace Atlas.Data.Tests.Rules
{
    [TestFixture]
    public class TopicRulesTests : TestFixtureBase
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
                var sut = new TopicRules(dbContext);
                var actual = await sut.IsValidAsync(category.SiteId, forum.Id, topic.Id);

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_false_when_topic_is_not_valid()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var sut = new TopicRules(dbContext);
                var actual = await sut.IsValidAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

                Assert.IsFalse(actual);
            }
        }
    }
}
