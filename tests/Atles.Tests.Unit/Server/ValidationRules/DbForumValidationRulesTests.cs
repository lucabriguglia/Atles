using Atles.Data;
using Atles.Domain;
using Atles.Server.ValidationRules;
using NUnit.Framework;

namespace Atles.Tests.Unit.Server.ValidationRules;

[TestFixture]
public class DbForumValidationRulesTests : TestFixtureBase
{
    [Test]
    public async Task Should_return_true_when_name_is_unique()
    {
        await using var dbContext = new AtlesDbContext(Shared.CreateContextOptions());
        var sut = new DbForumValidationRules(dbContext);
        var actual = await sut.IsForumNameUnique(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, "My Forum");
            
        Assert.IsTrue(actual);
    }

    [Test]
    public async Task Should_return_true_when_name_is_unique_for_existing_forum()
    {
        await using var dbContext = new AtlesDbContext(Shared.CreateContextOptions());
        var sut = new DbForumValidationRules(dbContext);
        var actual = await sut.IsForumNameUnique(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "My Forum");

        Assert.IsTrue(actual);
    }

    [Test]
    public async Task Should_return_false_when_name_is_not_unique()
    {
        var options = Shared.CreateContextOptions();

        var siteId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        const string forumName = "My Forum";

        await using (var dbContext = new AtlesDbContext(options))
        {
            var category = new Category(categoryId, siteId, "Category", 1, Guid.NewGuid());
            var forum = new Forum(categoryId, forumName, "My Forum", "my-forum", 1);
            dbContext.Categories.Add(category);
            dbContext.Forums.Add(forum);
            await dbContext.SaveChangesAsync();
        }

        await using (var dbContext = new AtlesDbContext(options))
        {
            var sut = new DbForumValidationRules(dbContext);
            var actual = await sut.IsForumNameUnique(siteId, categoryId, Guid.NewGuid(), "My Forum");

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

        await using (var dbContext = new AtlesDbContext(options))
        {
            var category = new Category(categoryId, siteId, "Category", 1, Guid.NewGuid());
            var forum1 = new Forum(categoryId, "Forum 1", "my-forum", "My Forum 1", 1);
            var forum2 = new Forum(forumId, categoryId, "Forum 2", "my-forum-2", "My Forum 2", 2);
            dbContext.Categories.Add(category);
            dbContext.Forums.Add(forum1);
            dbContext.Forums.Add(forum2);
            await dbContext.SaveChangesAsync();
        }

        await using (var dbContext = new AtlesDbContext(options))
        {
            var sut = new DbForumValidationRules(dbContext);
            var actual = await sut.IsForumNameUnique(siteId, categoryId, forumId, "Forum 1");

            Assert.IsFalse(actual);
        }
    }

    [Test]
    public async Task Should_return_true_when_slug_is_unique()
    {
        await using var dbContext = new AtlesDbContext(Shared.CreateContextOptions());
        var sut = new DbForumValidationRules(dbContext);
        var actual = await sut.IsForumSlugUnique(Guid.NewGuid(), Guid.Empty, "my-forum");

        Assert.IsTrue(actual);
    }

    [Test]
    public async Task Should_return_true_when_slug_is_unique_for_existing_forum()
    {
        await using var dbContext = new AtlesDbContext(Shared.CreateContextOptions());
        var sut = new DbForumValidationRules(dbContext);
        var actual = await sut.IsForumSlugUnique(Guid.NewGuid(), Guid.NewGuid(), "my-forum");

        Assert.IsTrue(actual);
    }

    [Test]
    public async Task Should_return_false_when_slug_is_not_unique()
    {
        var options = Shared.CreateContextOptions();

        var siteId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        const string forumSlug = "My Forum";

        await using (var dbContext = new AtlesDbContext(options))
        {
            var category = new Category(categoryId, siteId, "Category", 1, Guid.NewGuid());
            var forum = new Forum(categoryId, forumSlug, "My Forum", "my-forum", 1);
            dbContext.Categories.Add(category);
            dbContext.Forums.Add(forum);
            await dbContext.SaveChangesAsync();
        }

        await using (var dbContext = new AtlesDbContext(options))
        {
            var sut = new DbForumValidationRules(dbContext);
            var actual = await sut.IsForumSlugUnique(siteId, Guid.NewGuid(), "My Forum");

            Assert.IsFalse(actual);
        }
    }

    [Test]
    public async Task Should_return_false_when_slug_is_not_unique_for_existing_forum()
    {
        var options = Shared.CreateContextOptions();

        var siteId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var forumId = Guid.NewGuid();

        await using (var dbContext = new AtlesDbContext(options))
        {
            var category = new Category(categoryId, siteId, "Category", 1, Guid.NewGuid());
            var forum1 = new Forum(categoryId, "Forum 1", "my-forum", "My Forum 1", 1);
            var forum2 = new Forum(forumId, categoryId, "Forum 2", "my-forum-2", "My Forum 2", 2);
            dbContext.Categories.Add(category);
            dbContext.Forums.Add(forum1);
            dbContext.Forums.Add(forum2);
            await dbContext.SaveChangesAsync();
        }

        await using (var dbContext = new AtlesDbContext(options))
        {
            var sut = new DbForumValidationRules(dbContext);
            var actual = await sut.IsForumSlugUnique(siteId, forumId, "my-forum");

            Assert.IsFalse(actual);
        }
    }

    [Test]
    public async Task Should_return_true_when_forum_is_valid()
    {
        var options = Shared.CreateContextOptions();
        var category = new Category(Guid.NewGuid(), Guid.NewGuid(), "Category", 1, Guid.NewGuid());
        var forum = new Forum(Guid.NewGuid(), category.Id, "Forum", "my-forum", "My Forum", 1);

        await using (var dbContext = new AtlesDbContext(options))
        {
            dbContext.Categories.Add(category);
            dbContext.Forums.Add(forum);
            await dbContext.SaveChangesAsync();
        }

        await using (var dbContext = new AtlesDbContext(options))
        {
            var sut = new DbForumValidationRules(dbContext);
            var actual = await sut.IsForumValid(forum.Category.SiteId, forum.Id);

            Assert.IsTrue(actual);
        }
    }

    [Test]
    public async Task Should_return_false_when_forum_is_not_valid()
    {
        await using var dbContext = new AtlesDbContext(Shared.CreateContextOptions());
        var sut = new DbForumValidationRules(dbContext);
        var actual = await sut.IsForumValid(Guid.NewGuid(), Guid.NewGuid());

        Assert.IsFalse(actual);
    }
}
