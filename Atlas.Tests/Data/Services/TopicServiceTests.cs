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
            using (var dbContext = new AtlasDbContext(Shared.CreateContextOptions()))
            {
                var command = Fixture.Create<CreateTopic>();

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

                createValidator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.NotNull(topic);
                Assert.NotNull(@event);
            }
        }

        [Test]
        public async Task Should_update_topic_and_add_event()
        {
            var options = Shared.CreateContextOptions();
            var topic = Fixture.Create<Topic>();

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.Topics.Add(topic);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var command = Fixture.Build<UpdateTopic>()
                        .With(x => x.Id, topic.Id)
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
        public async Task Should_delete_topic_and_reorder_other_categories_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var siteId = Guid.NewGuid();

            var topic = new Topic(Guid.NewGuid(), Guid.NewGuid(), "Title", "Content", StatusType.Published);
            var reply1 = new Reply(topic.Id, Guid.NewGuid(), "Content", StatusType.Published);
            var reply2 = new Reply(topic.Id, Guid.NewGuid(), "Content", StatusType.Published);

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.Topics.Add(topic);

                dbContext.Replies.Add(reply1);
                dbContext.Replies.Add(reply2);

                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var command = Fixture.Build<DeleteTopic>()
                        .With(x => x.Id, topic.Id)
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

                var reply1Deleted = await dbContext.Replies.FirstOrDefaultAsync(x => x.Id == reply1.Id);
                var reply2Deleted = await dbContext.Replies.FirstOrDefaultAsync(x => x.Id == reply2.Id);
                var reply1Event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == reply1.Id);
                var reply2Event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == reply2.Id);

                Assert.AreEqual(StatusType.Deleted, topicDeleted.Status);
                Assert.NotNull(topicEvent);
                Assert.AreEqual(StatusType.Deleted, reply1Deleted.Status);
                Assert.AreEqual(StatusType.Deleted, reply2Deleted.Status);
                Assert.NotNull(reply1Event);
                Assert.NotNull(reply2Event);
            }
        }
    }
}
