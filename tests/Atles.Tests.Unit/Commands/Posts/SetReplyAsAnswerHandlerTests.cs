using Atles.Commands.Handlers.Posts;
using Atles.Commands.Posts;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Commands.Posts
{
    [TestFixture]
    public class SetReplyAsAnswerHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_set_reply_as_answer_and_add_event()
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
                var command = new SetReplyAsAnswer
                {
                    ReplyId = reply.Id,
                    SiteId = siteId,
                    ForumId = forumId,
                    TopicId = topicId,
                    IsAnswer = true
                };

                var cacheManager = new Mock<ICacheManager>();

                var sut = new SetReplyAsAnswerHandler(dbContext, cacheManager.Object);

                await sut.Handle(command);

                var replyUpdated = await dbContext.Posts.Include(x => x.Topic).FirstOrDefaultAsync(x => x.Id == reply.Id);
                var replyEvent = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == reply.Id);
                var userUpdated = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);

                Assert.True(replyUpdated.IsAnswer);
                Assert.True(replyUpdated.Topic.HasAnswer);
                Assert.NotNull(replyEvent);
                Assert.AreEqual(1, userUpdated.AnswersCount);
            }
        }

        [Test]
        public async Task Should_set_topic_as_not_answered_and_add_event()
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

            reply.SetAsAnswer(true);
            topic.SetAsAnswered(true);
            user.IncreaseAnswersCount();

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
                var command = new SetReplyAsAnswer
                {
                    ReplyId = reply.Id,
                    SiteId = siteId,
                    ForumId = forumId,
                    TopicId = topicId,
                    IsAnswer = false
                };

                var cacheManager = new Mock<ICacheManager>();

                var sut = new SetReplyAsAnswerHandler(dbContext, cacheManager.Object);

                await sut.Handle(command);

                var replyUpdated = await dbContext.Posts.Include(x => x.Topic).FirstOrDefaultAsync(x => x.Id == reply.Id);
                var replyEvent = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == reply.Id);
                var userUpdated = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);

                Assert.False(replyUpdated.IsAnswer);
                Assert.False(replyUpdated.Topic.HasAnswer);
                Assert.NotNull(replyEvent);
                Assert.AreEqual(0, userUpdated.AnswersCount);
            }
        }
    }
}
