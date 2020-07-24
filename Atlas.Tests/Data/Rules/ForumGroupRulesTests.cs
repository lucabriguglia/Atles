using Atlas.Data;
using Atlas.Data.Rules;
using Atlas.Domain;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Atlas.Domain.Categories;

namespace Atlas.Tests.Data.Rules
{
    [TestFixture]
    public class ForumGroupRulesTests : TestFixtureBase
    {
        [Test]
        public async Task Should_return_true_when_name_is_unique()
        {
            using (var dbContext = new AtlasDbContext(Shared.CreateContextOptions()))
            {
                var sut = new CategoryRules(dbContext);
                var actual = await sut.IsNameUniqueAsync(Guid.NewGuid(), "My Forum Group");

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_true_when_name_is_unique_for_existing_forum_group()
        {
            using (var dbContext = new AtlasDbContext(Shared.CreateContextOptions()))
            {
                var sut = new CategoryRules(dbContext);
                var actual = await sut.IsNameUniqueAsync(Guid.NewGuid(), "My Forum Group", Guid.NewGuid());

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_false_when_name_is_not_unique()
        {
            var options = Shared.CreateContextOptions();
            var siteId = Guid.NewGuid();
            var forumGroupName = "My Forum Group";

            using (var dbContext = new AtlasDbContext(options))
            {
                var forumGroup = new Category(siteId, forumGroupName, 1);
                dbContext.Categories.Add(forumGroup);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var sut = new CategoryRules(dbContext);
                var actual = await sut.IsNameUniqueAsync(siteId, forumGroupName);

                Assert.IsFalse(actual);
            }
        }

        [Test]
        public async Task Should_return_false_when_name_is_not_unique_for_existing_forum_group()
        {
            var options = Shared.CreateContextOptions();
            var siteId = Guid.NewGuid();
            var forumGroupId = Guid.NewGuid();

            using (var dbContext = new AtlasDbContext(options))
            {
                var forumGroup1 = new Category(siteId, "Forum Group 1", 1);
                var forumGroup2 = new Category(forumGroupId, siteId, "Forum Group 2", 2);
                dbContext.Categories.Add(forumGroup1);
                dbContext.Categories.Add(forumGroup2);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var sut = new CategoryRules(dbContext);
                var actual = await sut.IsNameUniqueAsync(siteId, "Forum Group 1", forumGroupId);

                Assert.IsFalse(actual);
            }
        }
    }
}
