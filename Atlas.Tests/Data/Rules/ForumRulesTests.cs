using Atlas.Data;
using Atlas.Data.Rules;
using Atlas.Domain;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Atlas.Domain.Forums;

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
                var actual = await sut.IsNameUniqueAsync(Guid.NewGuid(), "My Forum");

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_true_when_name_is_unique_for_existing_forum_group()
        {
            using (var dbContext = new AtlasDbContext(Shared.CreateContextOptions()))
            {
                var sut = new ForumRules(dbContext);
                var actual = await sut.IsNameUniqueAsync(Guid.NewGuid(), "My Forum", Guid.NewGuid());

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_false_when_name_is_not_unique()
        {
            var options = Shared.CreateContextOptions();
            var forumGroupId = Guid.NewGuid();
            var forumGroupName = "My Forum";

            using (var dbContext = new AtlasDbContext(options))
            {
                var forumGroup = new Forum(forumGroupId, forumGroupName, 1);
                dbContext.Forums.Add(forumGroup);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var sut = new ForumRules(dbContext);
                var actual = await sut.IsNameUniqueAsync(forumGroupId, forumGroupName);

                Assert.IsFalse(actual);
            }
        }

        [Test]
        public async Task Should_return_false_when_name_is_not_unique_for_existing_forum_group()
        {
            var options = Shared.CreateContextOptions();
            var forumGroupId = Guid.NewGuid();
            var forumId = Guid.NewGuid();

            using (var dbContext = new AtlasDbContext(options))
            {
                var forumGroup1 = new Forum(forumGroupId, "Forum 1", 1);
                var forumGroup2 = new Forum(forumId, forumGroupId, "Forum 2", 2);
                dbContext.Forums.Add(forumGroup1);
                dbContext.Forums.Add(forumGroup2);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var sut = new ForumRules(dbContext);
                var actual = await sut.IsNameUniqueAsync(forumGroupId, "Forum 1", forumId);

                Assert.IsFalse(actual);
            }
        }
    }
}
