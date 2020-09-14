using System;
using System.Threading;
using System.Threading.Tasks;
using Atlas.Data.Caching;
using Atlas.Data.Services;
using Atles.Domain.Categories;
using Atles.Domain.Forums;
using Atles.Domain.Posts;
using Atles.Domain.Posts.Commands;
using Atles.Domain.Users;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atlas.Data.Tests.Services
{
    [TestFixture]
    public class ReplyServiceTests : TestFixtureBase
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

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                dbContext.Posts.Add(topic);
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var command = Fixture.Build<CreateReply>()
                        .With(x => x.ForumId, forum.Id)
                        .With(x => x.TopicId, topicId)
                        .With(x => x.UserId, userId)
                    .Create();

                var cacheManager = new Mock<ICacheManager>();

                var createValidator = new Mock<IValidator<CreateReply>>();
                createValidator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var updateValidator = new Mock<IValidator<UpdateReply>>();

                var sut = new ReplyService(dbContext,
                    cacheManager.Object,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.CreateAsync(command);

                var reply = await dbContext.Posts.FirstOrDefaultAsync(x => x.Id == command.Id);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                var updatedCategory = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);
                var updatedForum = await dbContext.Forums.FirstOrDefaultAsync(x => x.Id == forum.Id);
                var updatedTopic = await dbContext.Posts.FirstOrDefaultAsync(x => x.Id == topic.Id);
                var updatedUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

                createValidator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
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

        [Test]
        public async Task Should_update_reply_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var siteId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var forumId = Guid.NewGuid();
            var topicId = Guid.NewGuid();

            var category = new Category(categoryId, siteId, "Category", 1, Guid.NewGuid());
            var forum = new Forum(forumId, categoryId, "Forum", "my-forum", "My Forum", 1);
            var topic = Post.CreateTopic(topicId, forumId, Guid.NewGuid(), "Title", "slug", "Content", PostStatusType.Published);
            var reply = Post.CreateReply(Guid.NewGuid(), topicId, forumId, Guid.NewGuid(), "Content", PostStatusType.Published);

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                dbContext.Posts.Add(topic);
                dbContext.Posts.Add(reply);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var command = Fixture.Build<UpdateReply>()
                        .With(x => x.Id, reply.Id)
                        .With(x => x.SiteId, siteId)
                        .With(x => x.ForumId, forumId)
                        .With(x => x.TopicId, topicId)
                    .Create();

                var cacheManager = new Mock<ICacheManager>();

                var createValidator = new Mock<IValidator<CreateReply>>();

                var updateValidator = new Mock<IValidator<UpdateReply>>();
                updateValidator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var sut = new ReplyService(dbContext,
                    cacheManager.Object,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.UpdateAsync(command);

                var updatedReply = await dbContext.Posts.FirstOrDefaultAsync(x => x.Id == command.Id);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                updateValidator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.AreEqual(command.Content, updatedReply.Content);
                Assert.NotNull(@event);
            }
        }

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

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                dbContext.Posts.Add(topic);
                dbContext.Posts.Add(reply);
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var command = new SetReplyAsAnswer
                {
                    Id = reply.Id,
                    SiteId = siteId,
                    ForumId = forumId,
                    TopicId = topicId,
                    IsAnswer = true
                };

                var cacheManager = new Mock<ICacheManager>();
                var createValidator = new Mock<IValidator<CreateReply>>();
                var updateValidator = new Mock<IValidator<UpdateReply>>();

                var sut = new ReplyService(dbContext,
                    cacheManager.Object,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.SetAsAnswerAsync(command);

                var replyUpdated = await dbContext.Posts.Include(x => x.Topic).FirstOrDefaultAsync(x => x.Id == reply.Id);
                var replyEvent = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == reply.Id);

                Assert.AreEqual(true, replyUpdated.IsAnswer);
                Assert.AreEqual(true, replyUpdated.Topic.HasAnswer);
                Assert.NotNull(replyEvent);
            }
        }

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

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                dbContext.Posts.Add(topic);
                dbContext.Posts.Add(reply);
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var command = Fixture.Build<DeleteReply>()
                        .With(x => x.Id, reply.Id)
                        .With(x => x.SiteId, siteId)
                        .With(x => x.ForumId, forumId)
                        .With(x => x.TopicId, topicId)
                    .Create();

                var cacheManager = new Mock<ICacheManager>();
                var createValidator = new Mock<IValidator<CreateReply>>();
                var updateValidator = new Mock<IValidator<UpdateReply>>();

                var sut = new ReplyService(dbContext,
                    cacheManager.Object,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.DeleteAsync(command);

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
