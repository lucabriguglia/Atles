using Atles.Data;
using Atles.Domain;
using Atles.Server.ValidationRules;
using NUnit.Framework;

namespace Atles.Tests.Unit.Server.ValidationRules;

[TestFixture]
public class CategoryValidationRulesTests : TestFixtureBase
{
    [Test]
    public async Task Should_return_true_when_name_is_unique()
    {
        await using var dbContext = new AtlesDbContext(Shared.CreateContextOptions());
        var sut = new CategoryValidationRules(dbContext);
        var actual = await sut.IsCategoryNameUnique(Guid.NewGuid(), "My Category");

        Assert.IsTrue(actual);
    }

    [Test]
    public async Task Should_return_true_when_name_is_unique_for_existing_category()
    {
        await using var dbContext = new AtlesDbContext(Shared.CreateContextOptions());
        var sut = new CategoryValidationRules(dbContext);
        var actual = await sut.IsCategoryNameUnique(Guid.NewGuid(), "My Category", Guid.NewGuid());

        Assert.IsTrue(actual);
    }

    [Test]
    public async Task Should_return_false_when_name_is_not_unique()
    {
        var options = Shared.CreateContextOptions();
        var siteId = Guid.NewGuid();
        const string categoryName = "My Category";

        await using (var dbContext = new AtlesDbContext(options))
        {
            var category = new Category(siteId, categoryName, 1, Guid.NewGuid());
            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();
        }

        await using (var dbContext = new AtlesDbContext(options))
        {
            var sut = new CategoryValidationRules(dbContext);
            var actual = await sut.IsCategoryNameUnique(siteId, categoryName);

            Assert.IsFalse(actual);
        }
    }

    [Test]
    public async Task Should_return_false_when_name_is_not_unique_for_existing_category()
    {
        var options = Shared.CreateContextOptions();
        var siteId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        await using (var dbContext = new AtlesDbContext(options))
        {
            var category1 = new Category(siteId, "Category 1", 1, Guid.NewGuid());
            var category2 = new Category(categoryId, siteId, "Category 2", 2, Guid.NewGuid());
            dbContext.Categories.Add(category1);
            dbContext.Categories.Add(category2);
            await dbContext.SaveChangesAsync();
        }

        await using (var dbContext = new AtlesDbContext(options))
        {
            var sut = new CategoryValidationRules(dbContext);
            var actual = await sut.IsCategoryNameUnique(siteId, "Category 1", categoryId);

            Assert.IsFalse(actual);
        }
    }
}