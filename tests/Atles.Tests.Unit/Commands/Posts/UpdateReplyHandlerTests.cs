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
    public class UpdateReplyHandlerTests : TestFixtureBase
    {
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

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                dbContext.Posts.Add(topic);
                dbContext.Posts.Add(reply);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var command = Fixture.Build<UpdateReply>()
                        .With(x => x.ReplyId, reply.Id)
                        .With(x => x.SiteId, siteId)
                        .With(x => x.ForumId, forumId)
                        .With(x => x.TopicId, topicId)
                    .Create();

                var cacheManager = new Mock<ICacheManager>();

                var validator = new Mock<IValidator<UpdateReply>>();
                validator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var sut = new UpdateReplyHandler(dbContext, validator.Object, cacheManager.Object);

                await sut.Handle(command);

                var updatedReply = await dbContext.Posts.FirstOrDefaultAsync(x => x.Id == command.ReplyId);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.ReplyId);

                validator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.AreEqual(command.Content, updatedReply.Content);
                Assert.NotNull(@event);
            }
        }
    }
}
