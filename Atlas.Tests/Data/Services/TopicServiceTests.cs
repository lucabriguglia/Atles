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
using Atlas.Domain.Topics;
using Atlas.Domain.Topics.Commands;
using TopicService = Atlas.Data.Services.TopicService;

namespace Atlas.Tests.Data.Services
{
    [TestFixture]
    public class TopicServiceTests : TestFixtureBase
    {
        [Test]
        public async Task Should_create_new_topic_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var categoryId = Guid.NewGuid();
            var forumId = Guid.NewGuid();

            var category = new Category(categoryId, Guid.NewGuid(), "Category", 1, Guid.NewGuid());
            var forum = new Forum(forumId, category.Id, "Forum", "My Forum", 1);

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var command = Fixture.Build<CreateTopic>().With(x => x.ForumId, forum.Id).Create();

                var cacheManager = new Mock<ICacheManager>();

                var createValidator = new Mock<IValidator<CreateTopic>>();
                createValidator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var updateValidator = new Mock<IValidator<UpdateTopic>>();

                var sut = new TopicService(dbContext,
                    cacheManager.Object,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.CreateAsync(command);

                var topic = await dbContext.Topics.FirstOrDefaultAsync(x => x.Id == command.Id);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                var updatedCategory = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);
                var updatedForum = await dbContext.Forums.FirstOrDefaultAsync(x => x.Id == forum.Id);

                createValidator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.NotNull(topic);
                Assert.NotNull(@event);
                Assert.AreEqual(category.TopicsCount + 1, updatedCategory.TopicsCount);
                Assert.AreEqual(forum.TopicsCount + 1, updatedForum.TopicsCount);
            }
        }

        [Test]
        public async Task Should_update_topic_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var siteId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var forumId = Guid.NewGuid();

            var category = new Category(categoryId, siteId, "Category", 1, Guid.NewGuid());
            var forum = new Forum(forumId, category.Id, "Forum", "My Forum", 1);
            var topic = new Topic(forumId, Guid.NewGuid(), "Title", "Content", StatusType.Published);

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                dbContext.Topics.Add(topic);

                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var command = Fixture.Build<UpdateTopic>()
                    .With(x => x.Id, topic.Id)
                    .With(x => x.ForumId, forumId)
                    .With(x => x.SiteId, siteId)
                    .Create();

                var cacheManager = new Mock<ICacheManager>();

                var createValidator = new Mock<IValidator<CreateTopic>>();

                var updateValidator = new Mock<IValidator<UpdateTopic>>();
                updateValidator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var sut = new TopicService(dbContext,
                    cacheManager.Object,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.UpdateAsync(command);

                var updatedTopic = await dbContext.Topics.FirstOrDefaultAsync(x => x.Id == command.Id);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                updateValidator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.AreEqual(command.Title, updatedTopic.Title);
                Assert.NotNull(@event);
            }
        }

        [Test]
        public async Task Should_delete_topic_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var siteId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var forumId = Guid.NewGuid();

            var category = new Category(categoryId, siteId, "Category", 1, Guid.NewGuid());
            var forum = new Forum(forumId, category.Id, "Forum", "My Forum", 1);
            var topic = new Topic(forumId, Guid.NewGuid(), "Title", "Content", StatusType.Published);

            category.IncreaseTopicsCount();
            forum.IncreaseTopicsCount();

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                dbContext.Topics.Add(topic);

                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var command = Fixture.Build<DeleteTopic>()
                        .With(x => x.Id, topic.Id)
                        .With(x => x.ForumId, forumId)
                        .With(x => x.SiteId, siteId)
                    .Create();

                var cacheManager = new Mock<ICacheManager>();
                var createValidator = new Mock<IValidator<CreateTopic>>();
                var updateValidator = new Mock<IValidator<UpdateTopic>>();

                var sut = new TopicService(dbContext,
                    cacheManager.Object,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.DeleteAsync(command);

                var topicDeleted = await dbContext.Topics.FirstOrDefaultAsync(x => x.Id == topic.Id);
                var topicEvent = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == topic.Id);

                var updatedCategory = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);
                var updatedForum = await dbContext.Forums.FirstOrDefaultAsync(x => x.Id == forum.Id);

                Assert.AreEqual(StatusType.Deleted, topicDeleted.Status);
                Assert.NotNull(topicEvent);
                Assert.AreEqual(0, updatedCategory.TopicsCount);
                Assert.AreEqual(0, updatedForum.TopicsCount);
            }
        }
    }
}
