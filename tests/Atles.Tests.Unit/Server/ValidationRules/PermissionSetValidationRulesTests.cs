using Atles.Data;
using Atles.Domain;
using Atles.Server.ValidationRules;
using NUnit.Framework;

namespace Atles.Tests.Unit.Server.ValidationRules;

[TestFixture]
public class PermissionSetValidationRulesTests : TestFixtureBase
{
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
            var sut = new PermissionSetValidationRules(dbContext);
            var actual = await sut.IsPermissionSetValid(site.Id, permissionSet.Id);

            Assert.IsTrue(actual);
        }
    }

    [Test]
    public async Task Should_return_false_when_permission_set_is_not_valid()
    {
        await using var dbContext = new AtlesDbContext(Shared.CreateContextOptions());
        var sut = new PermissionSetValidationRules(dbContext);
        var actual = await sut.IsPermissionSetValid(Guid.NewGuid(), Guid.NewGuid());

        Assert.IsFalse(actual);
    }
}