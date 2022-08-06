using Atles.Data;
using Atles.Domain;
using Atles.Server.ValidationRules;
using NUnit.Framework;

namespace Atles.Tests.Unit.Server.ValidationRules;

[TestFixture]
public class DbPermissionSetValidationRulesTests : TestFixtureBase
{
    [Test]
    public async Task Should_return_true_when_name_is_unique()
    {
        await using var dbContext = new AtlesDbContext(Shared.CreateContextOptions());
        var sut = new DbPermissionSetValidationRules(dbContext);
        var actual = await sut.IsPermissionSetNameUnique(Guid.NewGuid(), Guid.NewGuid(), "My Permission Set");

        Assert.IsTrue(actual);
    }

    [Test]
    public async Task Should_return_false_when_name_is_not_unique()
    {
        var options = Shared.CreateContextOptions();

        var siteId = Guid.NewGuid();
        const string permissionSetName = "My Permission Set";

        await using (var dbContext = new AtlesDbContext(options))
        {
            var permissionSet = new PermissionSet(Guid.NewGuid(), siteId, permissionSetName, new List<Permission>());
            dbContext.PermissionSets.Add(permissionSet);
            await dbContext.SaveChangesAsync();
        }

        await using (var dbContext = new AtlesDbContext(options))
        {
            var sut = new DbPermissionSetValidationRules(dbContext);
            var actual = await sut.IsPermissionSetNameUnique(siteId, Guid.NewGuid(), permissionSetName);

            Assert.IsFalse(actual);
        }
    }

    [Test]
    public async Task Should_return_false_when_name_is_not_unique_for_existing_permission_set()
    {
        var options = Shared.CreateContextOptions();

        var siteId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var permissionSetId = Guid.NewGuid();

        await using (var dbContext = new AtlesDbContext(options))
        {
            var permissionSet1 = new PermissionSet(Guid.NewGuid(), siteId, "Permission Set 1", new List<Permission>());
            var permissionSet2 = new PermissionSet(Guid.NewGuid(), siteId, "Permission Set 2", new List<Permission>());
            dbContext.PermissionSets.Add(permissionSet1);
            dbContext.PermissionSets.Add(permissionSet2);
            await dbContext.SaveChangesAsync();
        }

        await using (var dbContext = new AtlesDbContext(options))
        {
            var sut = new DbPermissionSetValidationRules(dbContext);
            var actual = await sut.IsPermissionSetNameUnique(siteId, permissionSetId, "Permission Set 1");

            Assert.IsFalse(actual);
        }
    }

    [Test]
    public async Task Should_return_true_when_permission_set_is_valid()
    {
        var options = Shared.CreateContextOptions();
        var site = new Site(Guid.NewGuid(), "Name", "Title");
        var permissionSet = new PermissionSet(Guid.NewGuid(), site.Id, "Permission Set", new List<Permission>());

        await using (var dbContext = new AtlesDbContext(options))
        {
            dbContext.Sites.Add(site);
            dbContext.PermissionSets.Add(permissionSet);
            await dbContext.SaveChangesAsync();
        }

        await using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
        {
            var sut = new DbPermissionSetValidationRules(dbContext);
            var actual = await sut.IsPermissionSetValid(site.Id, permissionSet.Id);

            Assert.IsTrue(actual);
        }
    }

    [Test]
    public async Task Should_return_false_when_permission_set_is_not_valid()
    {
        await using var dbContext = new AtlesDbContext(Shared.CreateContextOptions());
        var sut = new DbPermissionSetValidationRules(dbContext);
        var actual = await sut.IsPermissionSetValid(Guid.NewGuid(), Guid.NewGuid());

        Assert.IsFalse(actual);
    }
}