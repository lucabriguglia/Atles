using Atles.Data;
using Atles.Domain;
using Atles.Server.ValidationRules;
using NUnit.Framework;

namespace Atles.Tests.Unit.Server.ValidationRules;

[TestFixture]
public class DbCategoryValidationRulesTests : TestFixtureBase
{
    [Test]
    public async Task Should_return_true_when_name_is_unique()
    {
        await using var dbContext = new AtlesDbContext(Shared.CreateContextOptions());
        var sut = new DbCategoryValidationRules(dbContext);
        var actual = await sut.IsCategoryNameUnique(Guid.NewGuid(), Guid.NewGuid(), "My Category");

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
            var sut = new DbCategoryValidationRules(dbContext);
            var actual = await sut.IsCategoryNameUnique(siteId, Guid.NewGuid(), categoryName);

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
            var sut = new DbCategoryValidationRules(dbContext);
            var actual = await sut.IsCategoryNameUnique(siteId, categoryId, "Category 1");

            Assert.IsFalse(actual);
        }
    }

    [Test]
    public async Task Should_return_false_when_category_is_not_valid()
    {
        await using var dbContext = new AtlesDbContext(Shared.CreateContextOptions());
        var sut = new DbCategoryValidationRules(dbContext);
        var actual = await sut.IsCategoryValid(Guid.NewGuid(), Guid.NewGuid());

        Assert.IsFalse(actual);
    }

    [Test]
    public async Task Should_return_true_when_category_is_valid()
    {
        var options = Shared.CreateContextOptions();

        var siteId = Guid.NewGuid();
        var category = new Category(siteId, "Category Name", 1, Guid.NewGuid());

        await using (var dbContext = new AtlesDbContext(options))
        {
            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();
        }

        await using (var dbContext = new AtlesDbContext(options))
        {
            var sut = new DbCategoryValidationRules(dbContext);
            var actual = await sut.IsCategoryValid(siteId, category.Id);

            Assert.IsTrue(actual);
        }
    }
}