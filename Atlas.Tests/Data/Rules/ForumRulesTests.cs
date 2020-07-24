using Atlas.Data;
using Atlas.Data.Rules;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Atlas.Domain.Categories;
using Atlas.Domain.Forums;
using AutoFixture;

namespace Atlas.Tests.Data.Rules
{
    [TestFixture]
    public class ForumRulesTests : TestFixtureBase
    {
        [Test]
        public async Task Should_return_true_when_name_is_unique()
        {
            using (var dbContext = new AtlasDbContext(Shared.CreateContextOptions()))
            {
                var sut = new ForumRules(dbContext);
                var actual = await sut.IsNameUniqueAsync(Guid.NewGuid(), Guid.NewGuid(), "My Forum");

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_true_when_name_is_unique_for_existing_forum()
        {
            using (var dbContext = new AtlasDbContext(Shared.CreateContextOptions()))
            {
                var sut = new ForumRules(dbContext);
                var actual = await sut.IsNameUniqueAsync(Guid.NewGuid(), Guid.NewGuid(), "My Forum", Guid.NewGuid());

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

            using (var dbContext = new AtlasDbContext(options))
            {
                var category = new Category(categoryId, siteId, "Category", 1);
                var forum = new Forum(categoryId, forumName, 1);
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var sut = new ForumRules(dbContext);
                var actual = await sut.IsNameUniqueAsync(siteId, categoryId, forumName);

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

            using (var dbContext = new AtlasDbContext(options))
            {
                var category = new Category(categoryId, siteId, "Category", 1);
                var forum1 = new Forum(categoryId, "Forum 1", 1);
                var forum2 = new Forum(forumId, categoryId, "Forum 2", 2);
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum1);
                dbContext.Forums.Add(forum2);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var sut = new ForumRules(dbContext);
                var actual = await sut.IsNameUniqueAsync(siteId, categoryId, "Forum 1", forumId);

                Assert.IsFalse(actual);
            }
        }

        [Test]
        public async Task Should_return_true_when_it_is_valid()
        {
            var options = Shared.CreateContextOptions();
            var category = new Category(Guid.NewGuid(), Guid.NewGuid(), "Category", 1);
            var forum = new Forum(Guid.NewGuid(), category.Id, "Forum", 1);

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(Shared.CreateContextOptions()))
            {
                var sut = new ForumRules(dbContext);
                var actual = await sut.IsValidAsync(forum.Category.SiteId, forum.Id);

                Assert.IsTrue(actual);
            }
        }
    }
}
