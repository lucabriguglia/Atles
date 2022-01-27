using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain.Handlers.Posts.Commands;
using Atles.Domain.Models.Categories;
using Atles.Domain.Models.Forums;
using Atles.Domain.Models.Posts;
using Atles.Domain.Models.Posts.Commands;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Atles.Domain.Handlers.Tests.Posts.Commands
{
    [TestFixture]
    public class AddReactionHandlerTests : TestFixtureBase
    {
        [Test]
        public void Should_throw_data_exception_when_post_not_found()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var sut = new AddReactionHandler(dbContext);
                Assert.ThrowsAsync<DataException>(async () => await sut.Handle(Fixture.Create<AddReaction>()));
            }
        }

        [Test]
        public async Task Should_add_reaction()
        {
            var options = Shared.CreateContextOptions();

            var siteId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var forumId = Guid.NewGuid();
            var topicId = Guid.NewGuid();

            var category = new Category(categoryId, siteId, "Category", 1, Guid.NewGuid());
            var forum = new Forum(forumId, categoryId, "Forum", "my-forum", "My Forum", 1);
            var topic = Post.CreateTopic(topicId, forumId, Guid.NewGuid(), "Title", "slug", "Content", PostStatusType.Published);

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                dbContext.Posts.Add(topic);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var command = Fixture.Build<AddReaction>()
                        .With(x => x.Id, topic.Id)
                        .With(x => x.SiteId, siteId)
                    .Create();

                var sut = new AddReactionHandler(dbContext);

                await sut.Handle(command);

                var updatedPost = await dbContext.Posts.Include(x => x.PostReactions).FirstOrDefaultAsync(x => x.Id == command.Id);
                var postReaction = updatedPost.PostReactions.FirstOrDefault(x => x.Type == command.Type);

                Assert.AreEqual(1, postReaction.Count);
            }
        }
    }
}
