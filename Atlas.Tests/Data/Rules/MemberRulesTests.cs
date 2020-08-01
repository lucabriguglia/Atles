using Atlas.Data;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Atlas.Data.Rules;
using Atlas.Domain.Members;

namespace Atlas.Tests.Data.Rules
{
    [TestFixture]
    public class MemberRulesTests : TestFixtureBase
    {
        [Test]
        public async Task Should_return_true_when_display_name_is_unique()
        {
            using (var dbContext = new AtlasDbContext(Shared.CreateContextOptions()))
            {
                var sut = new MemberRules(dbContext);
                var actual = await sut.IsDisplayNameUniqueAsync("Display Name");

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_true_when_display_name_is_unique_for_existing_member()
        {
            using (var dbContext = new AtlasDbContext(Shared.CreateContextOptions()))
            {
                var sut = new MemberRules(dbContext);
                var actual = await sut.IsDisplayNameUniqueAsync("Display Name", Guid.NewGuid());

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_false_when_display_name_is_not_unique()
        {
            var options = Shared.CreateContextOptions();
            var displayName = "Display Name";

            using (var dbContext = new AtlasDbContext(options))
            {
                var member = new Member(displayName);
                dbContext.Categories.Add(category);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var sut = new MemberRules(dbContext);
                var actual = await sut.IsNameUniqueAsync(siteId, displayName);

                Assert.IsFalse(actual);
            }
        }

        [Test]
        public async Task Should_return_false_when_display_name_is_not_unique_for_existing_member()
        {
            var options = Shared.CreateContextOptions();
            var siteId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            using (var dbContext = new AtlasDbContext(options))
            {
                var category1 = new Member(siteId, "Member 1", 1, Guid.NewGuid());
                var category2 = new Member(categoryId, siteId, "Member 2", 2, Guid.NewGuid());
                dbContext.Categories.Add(category1);
                dbContext.Categories.Add(category2);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var sut = new MemberRules(dbContext);
                var actual = await sut.IsNameUniqueAsync(siteId, "Member 1", categoryId);

                Assert.IsFalse(actual);
            }
        }
    }
}
