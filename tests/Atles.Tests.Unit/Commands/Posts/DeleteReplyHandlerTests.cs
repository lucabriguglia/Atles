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
    public class DeleteReplyHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_delete_reply_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var siteId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var forumId = Guid.NewGuid();
            var topicId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var category = new Category(categoryId, siteId, "Category", 1, Guid.NewGuid());
            var forum = new Forum(forumId, categoryId, "Forum", "my-forum", "My Forum", 1);
            var topic = Post.CreateTopic(topicId, forumId, Guid.NewGuid(), "Title", "slug", "Content", PostStatusType.Published);
            var reply = Post.CreateReply(Guid.NewGuid(), topicId, forumId, userId, "Content", PostStatusType.Published);
            var user = new User(userId, Guid.NewGuid().ToString(), "Email", "Display Name");

            category.IncreaseRepliesCount();
            forum.IncreaseRepliesCount();
            topic.IncreaseRepliesCount();
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
                var command = Fixture.Build<DeleteReply>()
                        .With(x => x.ReplyId, reply.Id)
                        .With(x => x.SiteId, siteId)
                        .With(x => x.ForumId, forumId)
                        .With(x => x.TopicId, topicId)
                    .Create();

                var cacheManager = new Mock<ICacheManager>();

                var sut = new DeleteReplyHandler(dbContext, cacheManager.Object);

                await sut.Handle(command);

                var replyDeleted = await dbContext.Posts.FirstOrDefaultAsync(x => x.Id == reply.Id);
                var replyEvent = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == reply.Id);

                var updatedCategory = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);
                var updatedForum = await dbContext.Forums.FirstOrDefaultAsync(x => x.Id == forum.Id);
                var updatedTopic = await dbContext.Posts.FirstOrDefaultAsync(x => x.Id == topic.Id);
                var updatedUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);

                Assert.AreEqual(PostStatusType.Deleted, replyDeleted.Status);
                Assert.NotNull(replyEvent);
                Assert.AreEqual(0, updatedCategory.RepliesCount);
                Assert.AreEqual(0, updatedForum.RepliesCount);
                Assert.AreEqual(0, updatedTopic.RepliesCount);
                Assert.AreEqual(0, updatedUser.RepliesCount);
            }
        }
    }
}
