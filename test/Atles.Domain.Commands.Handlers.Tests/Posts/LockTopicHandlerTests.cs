﻿using Atles.Commands.Handlers.Posts;
using Atles.Commands.Posts;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain.Models;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Domain.Commands.Handlers.Tests.Posts
{
    [TestFixture]
    public class LockTopicHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_lock_topic_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var siteId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var forumId = Guid.NewGuid();

            var category = new Category(categoryId, siteId, "Category", 1, Guid.NewGuid());
            var forum = new Forum(forumId, category.Id, "Forum", "my-forum", "My Forum", 1);
            var topic = Post.CreateTopic(forumId, Guid.NewGuid(), "Title", "slug", "Content", PostStatusType.Published);

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                dbContext.Posts.Add(topic);

                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var command = Fixture.Build<LockTopic>()
                    .With(x => x.TopicId, topic.Id)
                    .With(x => x.ForumId, forumId)
                    .With(x => x.SiteId, siteId)
                    .Create();

                var cacheManager = new Mock<ICacheManager>();

                var sut = new LockTopicHandler(dbContext, cacheManager.Object);

                await sut.Handle(command);

                var lockedTopic = await dbContext.Posts.FirstOrDefaultAsync(x => x.Id == command.TopicId);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.TopicId);

                Assert.AreEqual(command.Locked, lockedTopic.Locked);
                Assert.NotNull(@event);
            }
        }
    }
}
