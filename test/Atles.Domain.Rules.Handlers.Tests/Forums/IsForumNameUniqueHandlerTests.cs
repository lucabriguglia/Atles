using Atles.Data;
using Atles.Domain.Rules.Forums;
using Atles.Domain.Rules.Handlers.Forums;
using NUnit.Framework;

namespace Atles.Domain.Rules.Handlers.Tests.Forums
{
    [TestFixture]
    public class IsForumNameUniqueHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_return_true_when_name_is_unique()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var sut = new IsForumNameUniqueHandler(dbContext);
                var query = new IsForumNameUnique 
                { 
                    SiteId = Guid.NewGuid(),
                    CategoryId = Guid.NewGuid(),
                    Name = "My Forum"
                };
                var actual = await sut.Handle(query);

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_true_when_name_is_unique_for_existing_forum()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var sut = new IsForumNameUniqueHandler(dbContext);
                var query = new IsForumNameUnique
                {
                    SiteId = Guid.NewGuid(),
                    CategoryId = Guid.NewGuid(),
                    Name = "My Forum",
                    Id = Guid.NewGuid()
                };
                var actual = await sut.Handle(query);

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_false_when_name_is_not_unique()
        {
            var options = Shared.CreateContextOptions();
            var siteId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            const string forumName = "My Forum";

            using (var dbContext = new AtlesDbContext(options))
            {
                var category = new Category(categoryId, siteId, "Category", 1, Guid.NewGuid());
                var forum = new Forum(categoryId, forumName, "My Forum", "my-forum", 1);
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var sut = new IsForumNameUniqueHandler(dbContext);
                var query = new IsForumNameUnique
                {
                    SiteId = siteId,
                    CategoryId = categoryId,
                    Name = forumName
                };
                var actual = await sut.Handle(query);

                Assert.IsFalse(actual);
            }
        }

        [Test]
        public async Task Should_return_false_when_name_is_not_unique_for_existing_forum()
        {
            var options = Shared.CreateContextOptions();
            var siteId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var forumId = Guid.NewGuid();

            using (var dbContext = new AtlesDbContext(options))
            {
                var category = new Category(categoryId, siteId, "Category", 1, Guid.NewGuid());
                var forum1 = new Forum(categoryId, "Forum 1", "My Forum", "my-forum", 1);
                var forum2 = new Forum(forumId, categoryId, "Forum 2", "my-forum-2", "My Forum", 2);
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum1);
                dbContext.Forums.Add(forum2);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var sut = new IsForumNameUniqueHandler(dbContext);
                var query = new IsForumNameUnique
                {
                    SiteId = siteId,
                    CategoryId = categoryId,
                    Name = "Forum 1",
                    Id = forumId
                };
                var actual = await sut.Handle(query);

                Assert.IsFalse(actual);
            }
        }

        [Test]
        public async Task Should_return_true_when_forum_is_valid()
        {
            var options = Shared.CreateContextOptions();
            var category = new Category(Guid.NewGuid(), Guid.NewGuid(), "Category", 1, Guid.NewGuid());
            var forum = new Forum(Guid.NewGuid(), category.Id, "Forum", "my-forum", "My Forum", 1);

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var sut = new IsForumValidHandler(dbContext);
                var query = new IsForumValid
                {
                    SiteId = forum.Category.SiteId,
                    Id = forum.Id
                };
                var actual = await sut.Handle(query);

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_false_when_forum_is_not_valid()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var sut = new IsForumValidHandler(dbContext);
                var query = new IsForumValid
                {
                    SiteId = Guid.NewGuid(),
                    Id = Guid.NewGuid()
                };
                var actual = await sut.Handle(query);

                Assert.IsFalse(actual);
            }
        }
    }
}
