using System;
using System.Threading.Tasks;
using Atles.Data.Rules;
using Atles.Domain.Themes;
using NUnit.Framework;

namespace Atles.Data.Tests.Rules
{
    [TestFixture]
    public class ThemeRulesTests : TestFixtureBase
    {
        [Test]
        public async Task Should_return_true_when_name_is_unique()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var sut = new ThemeRules(dbContext);
                var actual = await sut.IsNameUniqueAsync(Guid.NewGuid(), "My Theme");

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_true_when_name_is_unique_for_existing_entity()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var sut = new ThemeRules(dbContext);
                var actual = await sut.IsNameUniqueAsync(Guid.NewGuid(), "My Theme", Guid.NewGuid());

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_false_when_name_is_not_unique()
        {
            var options = Shared.CreateContextOptions();
            var siteId = Guid.NewGuid();
            const string name = "My Theme";

            using (var dbContext = new AtlesDbContext(options))
            {
                var category = Theme.CreateNew(siteId, name);
                dbContext.Themes.Add(category);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var sut = new ThemeRules(dbContext);
                var actual = await sut.IsNameUniqueAsync(siteId, name);

                Assert.IsFalse(actual);
            }
        }

        [Test]
        public async Task Should_return_false_when_name_is_not_unique_for_existing_entity()
        {
            var options = Shared.CreateContextOptions();
            var siteId = Guid.NewGuid();
            var id = Guid.NewGuid();

            using (var dbContext = new AtlesDbContext(options))
            {
                var entity1 = Theme.CreateNew(siteId, "Theme 1");
                var entity2 = Theme.CreateNew(id, siteId, "Theme 2");
                dbContext.Themes.Add(entity1);
                dbContext.Themes.Add(entity2);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var sut = new ThemeRules(dbContext);
                var actual = await sut.IsNameUniqueAsync(siteId, "Theme 1", id);

                Assert.IsFalse(actual);
            }
        }
    }
}