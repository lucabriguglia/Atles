using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain.Commands;
using Atles.Domain.Handlers.Posts.Commands;
using Atles.Domain.Models;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Atles.Domain.Handlers.Tests.Posts.Commands
{
    [TestFixture]
    public class RemoveReactionHandlerTests : TestFixtureBase
    {
        [Test]
        public void Should_throw_data_exception_when_post_not_found()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var sut = new RemoveReactionHandler(dbContext);
                Assert.ThrowsAsync<DataException>(async () => await sut.Handle(Fixture.Create<RemoveReaction>()));
            }
        }

        [Test]
        public async Task Should_remove_reaction()
        {
            var options = Shared.CreateContextOptions();

            var siteId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var forumId = Guid.NewGuid();
            var topicId = Guid.NewGuid();

            var category = new Category(categoryId, siteId, "Category", 1, Guid.NewGuid());
            var forum = new Forum(forumId, categoryId, "Forum", "my-forum", "My Forum", 1);
            var topic = Post.CreateTopic(topicId, forumId, Guid.NewGuid(), "Title", "slug", "Content", PostStatusType.Published);
            topic.AddReactionToSummary(PostReactionType.Support);
            var postReaction = new PostReaction(topic.Id, topic.CreatedBy, PostReactionType.Support);

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                dbContext.Posts.Add(topic);
                dbContext.PostReactions.Add(postReaction);

                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var command = new RemoveReaction
                {
                    Id = topic.Id,
                    ForumId = forumId,
                    SiteId = siteId,
                    UserId = postReaction.UserId
                };

                var sut = new RemoveReactionHandler(dbContext);

                await sut.Handle(command);

                var updatedPost = await dbContext.Posts.Include(x => x.PostReactionSummaries).FirstOrDefaultAsync(x => x.Id == command.Id);

                Assert.AreEqual(0, updatedPost.PostReactionSummaries.FirstOrDefault(x => x.Type == PostReactionType.Support).Count);
            }
        }
    }
}
