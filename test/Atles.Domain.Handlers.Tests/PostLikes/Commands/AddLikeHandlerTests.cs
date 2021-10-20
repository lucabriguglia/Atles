using System;
using System.Data;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain.Categories;
using Atles.Domain.Forums;
using Atles.Domain.Handlers.PostLikes.Commands;
using Atles.Domain.PostLikes.Commands;
using Atles.Domain.Posts;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Atles.Domain.Handlers.Tests.PostLikes.Commands
{
    [TestFixture]
    public class AddLikeHandlerTests : TestFixtureBase
    {
        [Test]
        public void Should_throw_data_exption_when_post_not_found()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var sut = new AddLikeHandler(dbContext);
                Assert.ThrowsAsync<DataException>(async () => await sut.Handle(Fixture.Create<AddLike>()));
            }
        }

        [Test]
        public async Task Should_add_like_and_increase_count()
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
                var command = Fixture.Build<AddLike>()
                        .With(x => x.PostId, topic.Id)
                        .With(x => x.SiteId, siteId)
                    .Create();

                var sut = new AddLikeHandler(dbContext);

                await sut.Handle(command);

                var updatedPost = await dbContext.Posts.FirstOrDefaultAsync(x => x.Id == command.PostId);
                var postLike = await dbContext.PostLikes.FirstOrDefaultAsync(x => x.PostId == command.PostId);

                Assert.AreEqual(1, updatedPost.LikesCount);
                Assert.NotNull(postLike);
            }
        }
    }
}
