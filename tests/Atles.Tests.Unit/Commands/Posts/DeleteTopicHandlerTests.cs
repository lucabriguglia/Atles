using Atles.Commands.Handlers.Posts;
using Atles.Commands.Posts;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Commands.Posts
{
    [TestFixture]
    public class DeleteTopicHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_delete_topic_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var siteId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var forumId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var category = new Category(categoryId, siteId, "Category", 1, Guid.NewGuid());
            var forum = new Forum(forumId, category.Id, "Forum", "my-forum", "My Forum", 1);
            var topic = Post.CreateTopic(forumId, userId, "Title", "slug", "Content", PostStatusType.Published);
            var reply = Post.CreateReply(topic.Id, forumId, userId, "Content", PostStatusType.Published);
            var user = new User(userId, Guid.NewGuid().ToString(), "Email", "Display Name");

            category.IncreaseTopicsCount();
            category.IncreaseRepliesCount();
            forum.IncreaseTopicsCount();
            forum.IncreaseRepliesCount();
            topic.IncreaseRepliesCount();
            user.IncreaseTopicsCount();
            user.IncreaseRepliesCount();

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                dbContext.Posts.Add(topic);
                dbContext.Posts.Add(reply);
                dbContext.Users.Add(user);

                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var command = Fixture.Build<DeleteTopic>()
                        .With(x => x.TopicId, topic.Id)
                        .With(x => x.ForumId, forumId)
                        .With(x => x.SiteId, siteId)
                    .Create();

                var cacheManager = new Mock<ICacheManager>();

                var sut = new DeleteTopicHandler(dbContext, cacheManager.Object);

                await sut.Handle(command);

                var topicDeleted = await dbContext.Posts.FirstOrDefaultAsync(x => x.Id == topic.Id);
                var topicEvent = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == topic.Id);

                var updatedCategory = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);
                var updatedForum = await dbContext.Forums.FirstOrDefaultAsync(x => x.Id == forum.Id);
                var updatedUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);

                Assert.AreEqual(PostStatusType.Deleted, topicDeleted.Status);
                Assert.NotNull(topicEvent);
                Assert.AreEqual(0, updatedCategory.TopicsCount, "Category topics count");
                Assert.AreEqual(0, updatedCategory.RepliesCount, "Category replies count");
                Assert.AreEqual(0, updatedForum.TopicsCount, "Forum topics count");
                Assert.AreEqual(0, updatedForum.RepliesCount, "Forum replies count");
                Assert.AreEqual(0, updatedUser.TopicsCount, "User topics count");
                Assert.AreEqual(0, updatedUser.RepliesCount, "User replies count");
            }
        }
    }
}
