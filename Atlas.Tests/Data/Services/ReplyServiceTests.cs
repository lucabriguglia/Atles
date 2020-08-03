using System;
using Atlas.Data;
using Atlas.Data.Caching;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using Atlas.Domain;
using Atlas.Domain.Categories;
using Atlas.Domain.Forums;
using Atlas.Domain.Replies;
using Atlas.Domain.Replies.Commands;
using Atlas.Domain.Topics;
using Atlas.Data.Services;
using Atlas.Domain.Members;

namespace Atlas.Tests.Data.Services
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
            var memberId = Guid.NewGuid();

            var category = new Category(categoryId, Guid.NewGuid(), "Category", 1, Guid.NewGuid());
            var forum = new Forum(forumId, category.Id, "Forum", "My Forum", 1);
            var topic = new Topic(topicId, forumId, Guid.NewGuid(), "Title", "Content", StatusType.Published);
            var member = new Member(memberId, Guid.NewGuid().ToString(), "Email", "Display Name");

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                dbContext.Topics.Add(topic);
                dbContext.Members.Add(member);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var command = Fixture.Build<CreateReply>()
                        .With(x => x.ForumId, forum.Id)
                        .With(x => x.TopicId, topicId)
                        .With(x => x.MemberId, memberId)
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

                var reply = await dbContext.Replies.FirstOrDefaultAsync(x => x.Id == command.Id);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                var updatedCategory = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);
                var updatedForum = await dbContext.Forums.FirstOrDefaultAsync(x => x.Id == forum.Id);
                var updatedTopic = await dbContext.Topics.FirstOrDefaultAsync(x => x.Id == topic.Id);
                var updatedMember = await dbContext.Members.FirstOrDefaultAsync(x => x.Id == memberId);

                createValidator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.NotNull(reply);
                Assert.NotNull(@event);
                Assert.AreEqual(category.RepliesCount + 1, updatedCategory.RepliesCount);
                Assert.AreEqual(forum.RepliesCount + 1, updatedForum.RepliesCount);
                Assert.AreEqual(topic.RepliesCount + 1, updatedTopic.RepliesCount);
                Assert.AreEqual(member.RepliesCount + 1, updatedMember.RepliesCount);
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
            var forum = new Forum(forumId, categoryId, "Forum", "My Forum", 1);
            var topic = new Topic(topicId, forumId, Guid.NewGuid(), "Title", "Content", StatusType.Published);
            var reply = new Reply(Guid.NewGuid(), topicId, Guid.NewGuid(), "Content", StatusType.Published);

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                dbContext.Topics.Add(topic);
                dbContext.Replies.Add(reply);
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

                var updatedReply = await dbContext.Replies.FirstOrDefaultAsync(x => x.Id == command.Id);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                updateValidator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.AreEqual(command.Content, updatedReply.Content);
                Assert.NotNull(@event);
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
            var memberId = Guid.NewGuid();

            var category = new Category(categoryId, siteId, "Category", 1, Guid.NewGuid());
            var forum = new Forum(forumId, categoryId, "Forum", "My Forum", 1);
            var topic = new Topic(topicId, forumId, Guid.NewGuid(), "Title", "Content", StatusType.Published);
            var reply = new Reply(Guid.NewGuid(), topicId, memberId, "Content", StatusType.Published);
            var member = new Member(memberId, Guid.NewGuid().ToString(), "Email", "Display Name");

            category.IncreaseRepliesCount();
            forum.IncreaseRepliesCount();
            topic.IncreaseRepliesCount();
            member.IncreaseRepliesCount();

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                dbContext.Topics.Add(topic);
                dbContext.Replies.Add(reply);
                dbContext.Members.Add(member);
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

                var replyDeleted = await dbContext.Replies.FirstOrDefaultAsync(x => x.Id == reply.Id);
                var replyEvent = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == reply.Id);

                var updatedCategory = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);
                var updatedForum = await dbContext.Forums.FirstOrDefaultAsync(x => x.Id == forum.Id);
                var updatedTopic = await dbContext.Topics.FirstOrDefaultAsync(x => x.Id == topic.Id);
                var updatedMember = await dbContext.Members.FirstOrDefaultAsync(x => x.Id == member.Id);

                Assert.AreEqual(StatusType.Deleted, replyDeleted.Status);
                Assert.NotNull(replyEvent);
                Assert.AreEqual(0, updatedCategory.RepliesCount);
                Assert.AreEqual(0, updatedForum.RepliesCount);
                Assert.AreEqual(0, updatedTopic.RepliesCount);
                Assert.AreEqual(0, updatedMember.RepliesCount);
            }
        }
    }
}
