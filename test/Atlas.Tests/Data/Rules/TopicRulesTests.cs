using Atlas.Data;
using Atlas.Data.Rules;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Atlas.Domain;
using Atlas.Domain.Categories;
using Atlas.Domain.Forums;
using Atlas.Domain.Topics;

namespace Atlas.Tests.Data.Rules
{
    [TestFixture]
    public class TopicRulesTests : TestFixtureBase
    {
        [Test]
        public async Task Should_return_true_when_topic_is_valid()
        {
            var options = Shared.CreateContextOptions();
            var category = new Category(Guid.NewGuid(), Guid.NewGuid(), "Category", 1, Guid.NewGuid());
            var forum = new Forum(Guid.NewGuid(), category.Id, "Forum", "My Forum", 1, Guid.NewGuid());
            var topic = new Topic(Guid.NewGuid(), forum.Id, Guid.NewGuid(), "Title", "Content", StatusType.Published);

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                dbContext.Topics.Add(topic);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var sut = new TopicRules(dbContext);
                var actual = await sut.IsValidAsync(category.SiteId, forum.Id, topic.Id);

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_false_when_topic_is_not_valid()
        {
            using (var dbContext = new AtlasDbContext(Shared.CreateContextOptions()))
            {
                var sut = new TopicRules(dbContext);
                var actual = await sut.IsValidAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

                Assert.IsFalse(actual);
            }
        }
    }
}
