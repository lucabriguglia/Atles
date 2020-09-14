using System;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Data.Rules;
using Atles.Domain.Categories;
using NUnit.Framework;

namespace Atlas.Data.Tests.Rules
{
    [TestFixture]
    public class CategoryRulesTests : TestFixtureBase
    {
        [Test]
        public async Task Should_return_true_when_name_is_unique()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var sut = new CategoryRules(dbContext);
                var actual = await sut.IsNameUniqueAsync(Guid.NewGuid(), "My Category");

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_true_when_name_is_unique_for_existing_category()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var sut = new CategoryRules(dbContext);
                var actual = await sut.IsNameUniqueAsync(Guid.NewGuid(), "My Category", Guid.NewGuid());

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_false_when_name_is_not_unique()
        {
            var options = Shared.CreateContextOptions();
            var siteId = Guid.NewGuid();
            var categoryName = "My Category";

            using (var dbContext = new AtlesDbContext(options))
            {
                var category = new Category(siteId, categoryName, 1, Guid.NewGuid());
                dbContext.Categories.Add(category);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var sut = new CategoryRules(dbContext);
                var actual = await sut.IsNameUniqueAsync(siteId, categoryName);

                Assert.IsFalse(actual);
            }
        }

        [Test]
        public async Task Should_return_false_when_name_is_not_unique_for_existing_category()
        {
            var options = Shared.CreateContextOptions();
            var siteId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            using (var dbContext = new AtlesDbContext(options))
            {
                var category1 = new Category(siteId, "Category 1", 1, Guid.NewGuid());
                var category2 = new Category(categoryId, siteId, "Category 2", 2, Guid.NewGuid());
                dbContext.Categories.Add(category1);
                dbContext.Categories.Add(category2);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var sut = new CategoryRules(dbContext);
                var actual = await sut.IsNameUniqueAsync(siteId, "Category 1", categoryId);

                Assert.IsFalse(actual);
            }
        }
    }
}
