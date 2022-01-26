using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain.Handlers.PostReactions.Commands;
using Atles.Domain.Models.Categories;
using Atles.Domain.Models.Forums;
using Atles.Domain.Models.PostReactions;
using Atles.Domain.Models.Posts;
using Atles.Domain.PostReactions.Commands;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Atles.Domain.Handlers.Tests.PostLikes.Commands
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
        public async Task Should_add_reaction_and_increase_count()
        {
            var options = Shared.CreateContextOptions();

            var siteId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var forumId = Guid.NewGuid();
            var topicId = Guid.NewGuid();

            var category = new Category(categoryId, siteId, "Category", 1, Guid.NewGuid());
            var forum = new Forum(forumId, categoryId, "Forum", "my-forum", "My Forum", 1);
            var topic = Post.CreateTopic(topicId, forumId, Guid.NewGuid(), "Title", "slug", "Content", PostStatusType.Published);
            var postReaction = new PostReaction(topicId, Guid.NewGuid(), PostReactionType.Support);

            topic.IncreaseReactionCount(PostReactionType.Support);

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
                    SiteId = siteId,
                    PostId = postReaction.PostId,
                    UserId = postReaction.UserId
                };

                var sut = new RemoveReactionHandler(dbContext);

                await sut.Handle(command);

                var updatedPost = await dbContext.Posts.Include(x => x.PostReactionCounts).FirstOrDefaultAsync(x => x.Id == command.PostId);
                var removedPostReaction = await dbContext.PostReactions.FirstOrDefaultAsync(x => x.PostId == command.PostId && x.UserId == command.UserId);

                Assert.AreEqual(0, updatedPost.PostReactionCounts.FirstOrDefault(x => x.Type == PostReactionType.Support).Count);
                Assert.Null(removedPostReaction);
            }
        }
    }
}
