using Atles.Commands.Handlers.Posts;
using Atles.Commands.Handlers.Posts.Services;
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
    public class CreateTopicHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_create_new_topic_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var categoryId = Guid.NewGuid();
            var forumId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var category = new Category(categoryId, Guid.NewGuid(), "Category", 1, Guid.NewGuid());
            var forum = new Forum(forumId, category.Id, "Forum", "my-forum", "My Forum", 1);
            var user = new User(userId, Guid.NewGuid().ToString(), "Email", "Display Name");

            await using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }

            await using (var dbContext = new AtlesDbContext(options))
            {
                var command = Fixture.Build<CreateTopic>()
                        .With(x => x.ForumId, forum.Id)
                        .With(x => x.UserId, userId)
                    .Create();

                var cacheManager = new Mock<ICacheManager>();

                var validator = new Mock<IValidator<CreateTopic>>();
                validator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var topicSlugGenerator = new Mock<ITopicSlugGenerator>();
                topicSlugGenerator
                    .Setup(x => x.GenerateTopicSlug(command.ForumId, command.Title))
                    .ReturnsAsync("slug");

                var sut = new CreateTopicHandler(dbContext, validator.Object, cacheManager.Object, topicSlugGenerator.Object);

                await sut.Handle(command);

                var topic = await dbContext.Posts.FirstOrDefaultAsync(x => x.Id == command.TopicId);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.TopicId);

                var updatedCategory = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);
                var updatedForum = await dbContext.Forums.FirstOrDefaultAsync(x => x.Id == forum.Id);
                var updatedUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

                validator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.NotNull(topic);
                Assert.NotNull(@event);
                Assert.AreEqual(category.TopicsCount + 1, updatedCategory.TopicsCount);
                Assert.AreEqual(topic.Id, updatedForum.LastPostId);
                Assert.AreEqual(forum.TopicsCount + 1, updatedForum.TopicsCount);
                Assert.AreEqual(forum.TopicsCount + 1, updatedForum.TopicsCount);
                Assert.AreEqual(user.TopicsCount + 1, updatedUser.TopicsCount);
            }
        }

        [Test]
        public async Task Should_add_subscription()
        {
            var options = Shared.CreateContextOptions();

            var category = new Category(Guid.NewGuid(), Guid.NewGuid(), "Category", 1, Guid.NewGuid());
            var forum = new Forum(Guid.NewGuid(), category.Id, "Forum", "my-forum", "My Forum", 1);
            var user = new User(Guid.NewGuid(), Guid.NewGuid().ToString(), "Email", "Display Name");

            await using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }

            await using (var dbContext = new AtlesDbContext(options))
            {
                var command = Fixture.Build<CreateTopic>()
                        .With(x => x.ForumId, forum.Id)
                        .With(x => x.UserId, user.Id)
                        .With(x => x.Subscribe, true)
                    .Create();

                var cacheManager = new Mock<ICacheManager>();

                var validator = new Mock<IValidator<CreateTopic>>();
                validator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var topicSlugGenerator = new Mock<ITopicSlugGenerator>();
                topicSlugGenerator
                    .Setup(x => x.GenerateTopicSlug(command.ForumId, command.Title))
                    .ReturnsAsync("slug");

                var sut = new CreateTopicHandler(dbContext, validator.Object, cacheManager.Object, topicSlugGenerator.Object);

                await sut.Handle(command);

                var subscription = await dbContext.Subscriptions
                    .FirstOrDefaultAsync(x => 
                        x.UserId == command.UserId && 
                        x.ItemId == command.TopicId && 
                        x.Type == SubscriptionType.Topic);

                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.TopicId);

                Assert.NotNull(subscription);
                Assert.NotNull(@event);
            }
        }

        [Test]
        public async Task Should_not_add_subscription()
        {
            var options = Shared.CreateContextOptions();

            var category = new Category(Guid.NewGuid(), Guid.NewGuid(), "Category", 1, Guid.NewGuid());
            var forum = new Forum(Guid.NewGuid(), category.Id, "Forum", "my-forum", "My Forum", 1);
            var user = new User(Guid.NewGuid(), Guid.NewGuid().ToString(), "Email", "Display Name");

            await using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }

            await using (var dbContext = new AtlesDbContext(options))
            {
                var command = Fixture.Build<CreateTopic>()
                        .With(x => x.ForumId, forum.Id)
                        .With(x => x.UserId, user.Id)
                        .With(x => x.Subscribe, false)
                    .Create();

                var cacheManager = new Mock<ICacheManager>();

                var validator = new Mock<IValidator<CreateTopic>>();
                validator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var topicSlugGenerator = new Mock<ITopicSlugGenerator>();
                topicSlugGenerator
                    .Setup(x => x.GenerateTopicSlug(command.ForumId, command.Title))
                    .ReturnsAsync("slug");

                var sut = new CreateTopicHandler(dbContext, validator.Object, cacheManager.Object, topicSlugGenerator.Object);

                await sut.Handle(command);

                var subscription = await dbContext.Subscriptions
                    .FirstOrDefaultAsync(x =>
                        x.UserId == command.UserId &&
                        x.ItemId == command.TopicId &&
                        x.Type == SubscriptionType.Topic);

                Assert.Null(subscription);
            }
        }
    }
}
