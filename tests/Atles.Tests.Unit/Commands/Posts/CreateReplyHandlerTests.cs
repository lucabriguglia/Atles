using Atles.Commands.Handlers.Posts;
using Atles.Commands.Posts;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Commands.Posts
{
    [TestFixture]
    public class CreateReplyHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_create_new_reply_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var categoryId = Guid.NewGuid();
            var forumId = Guid.NewGuid();
            var topicId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var category = new Category(categoryId, Guid.NewGuid(), "Category", 1, Guid.NewGuid());
            var forum = new Forum(forumId, category.Id, "Forum", "my-forum", "My Forum", 1);
            var topic = Post.CreateTopic(topicId, forumId, Guid.NewGuid(), "Title", "slug", "Content", PostStatusType.Published);
            var user = new User(userId, Guid.NewGuid().ToString(), "Email", "Display Name");

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                dbContext.Posts.Add(topic);
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var command = Fixture.Build<CreateReply>()
                        .With(x => x.ForumId, forum.Id)
                        .With(x => x.TopicId, topicId)
                        .With(x => x.UserId, userId)
                    .Create();

                var cacheManager = new Mock<ICacheManager>();

                var validator = new Mock<IValidator<CreateReply>>();
                validator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var sut = new CreateReplyHandler(dbContext, validator.Object, cacheManager.Object);

                await sut.Handle(command);

                var reply = await dbContext.Posts.FirstOrDefaultAsync(x => x.Id == command.ReplyId);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.ReplyId);

                var updatedCategory = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);
                var updatedForum = await dbContext.Forums.FirstOrDefaultAsync(x => x.Id == forum.Id);
                var updatedTopic = await dbContext.Posts.FirstOrDefaultAsync(x => x.Id == topic.Id);
                var updatedUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

                validator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.NotNull(reply);
                Assert.NotNull(@event);
                Assert.AreEqual(category.RepliesCount + 1, updatedCategory.RepliesCount);
                Assert.AreEqual(reply.Id, updatedForum.LastPostId);
                Assert.AreEqual(forum.RepliesCount + 1, updatedForum.RepliesCount);
                Assert.AreEqual(topic.RepliesCount + 1, updatedTopic.RepliesCount);
                Assert.AreEqual(reply.Id, updatedTopic.LastReplyId);
                Assert.AreEqual(user.RepliesCount + 1, updatedUser.RepliesCount);
            }
        }
    }
}
