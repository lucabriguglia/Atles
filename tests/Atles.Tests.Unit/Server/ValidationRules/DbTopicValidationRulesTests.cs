using Atles.Data;
using Atles.Domain;
using Atles.Server.ValidationRules;
using NUnit.Framework;

namespace Atles.Tests.Unit.Server.ValidationRules;

[TestFixture]
public class DbTopicValidationRulesTests : TestFixtureBase
{
    [Test]
    public async Task Should_return_true_when_topic_is_valid()
    {
        var options = Shared.CreateContextOptions();
        var category = new Category(Guid.NewGuid(), Guid.NewGuid(), "Category", 1, Guid.NewGuid());
        var forum = new Forum(Guid.NewGuid(), category.Id, "Forum", "my-forum", "My Forum", 1, Guid.NewGuid());
        var topic = Post.CreateTopic(Guid.NewGuid(), forum.Id, Guid.NewGuid(), "Title", "slug", "Content", PostStatusType.Published);

        await using (var dbContext = new AtlesDbContext(options))
        {
            dbContext.Categories.Add(category);
            dbContext.Forums.Add(forum);
            dbContext.Posts.Add(topic);
            await dbContext.SaveChangesAsync();
        }

        await using (var dbContext = new AtlesDbContext(options))
        {
            var sut = new DbTopicValidationRules(dbContext);
            var actual = await sut.IsTopicValid(category.SiteId, forum.Id, topic.Id);

            Assert.IsTrue(actual);
        }
    }

    [Test]
    public async Task Should_return_false_when_topic_is_not_valid()
    {
        await using var dbContext = new AtlesDbContext(Shared.CreateContextOptions());
        var sut = new DbTopicValidationRules(dbContext);
        var actual = await sut.IsTopicValid(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        Assert.IsFalse(actual);
    }
}
