using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Data.Rules;
using Atles.Domain.PermissionSets;
using Atles.Domain.PermissionSets.Commands;
using Atles.Domain.Sites;
using NUnit.Framework;

namespace Atlas.Data.Tests.Rules
{
    [TestFixture]
    public class PermissionSetRulesTests : TestFixtureBase
    {
        [Test]
        public async Task Should_return_true_when_name_is_unique()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var sut = new PermissionSetRules(dbContext);
                var actual = await sut.IsNameUniqueAsync(Guid.NewGuid(), "My Permission Set");

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_true_when_name_is_unique_for_existing_permission_set()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var sut = new PermissionSetRules(dbContext);
                var actual = await sut.IsNameUniqueAsync(Guid.NewGuid(), "My Permission Set", Guid.NewGuid());

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_false_when_name_is_not_unique()
        {
            var options = Shared.CreateContextOptions();
            var siteId = Guid.NewGuid();
            const string permissionSetName = "My Permission Set";

            using (var dbContext = new AtlesDbContext(options))
            {
                var site = new Site(siteId, "Name", "Title");
                var permissionSet = new PermissionSet(siteId, permissionSetName, new List<PermissionCommand>());
                dbContext.Sites.Add(site);
                dbContext.PermissionSets.Add(permissionSet);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var sut = new PermissionSetRules(dbContext);
                var actual = await sut.IsNameUniqueAsync(siteId, permissionSetName);

                Assert.IsFalse(actual);
            }
        }

        [Test]
        public async Task Should_return_false_when_name_is_not_unique_for_existing_permission_set()
        {
            var options = Shared.CreateContextOptions();
            var siteId = Guid.NewGuid();
            var permissionSetId = Guid.NewGuid();

            using (var dbContext = new AtlesDbContext(options))
            {
                var site = new Site(siteId, "Name", "Title");
                var permissionSet1 = new PermissionSet(siteId, "Permission Set 1", new List<PermissionCommand>());
                var permissionSet2 = new PermissionSet(permissionSetId, siteId, "Permission Set 2", new List<PermissionCommand>());
                dbContext.Sites.Add(site);
                dbContext.PermissionSets.Add(permissionSet1);
                dbContext.PermissionSets.Add(permissionSet2);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var sut = new PermissionSetRules(dbContext);
                var actual = await sut.IsNameUniqueAsync(siteId, "Permission Set 1", permissionSetId);

                Assert.IsFalse(actual);
            }
        }

        [Test]
        public async Task Should_return_true_when_permission_set_is_valid()
        {
            var options = Shared.CreateContextOptions();
            var site = new Site(Guid.NewGuid(), "Name", "Title");
            var permissionSet = new PermissionSet(Guid.NewGuid(), site.Id, "Permission Set", new List<PermissionCommand>());

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Sites.Add(site);
                dbContext.PermissionSets.Add(permissionSet);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var sut = new PermissionSetRules(dbContext);
                var actual = await sut.IsValidAsync(site.Id, permissionSet.Id);

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_false_when_permission_set_is_not_valid()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var sut = new PermissionSetRules(dbContext);
                var actual = await sut.IsValidAsync(Guid.NewGuid(), Guid.NewGuid());

                Assert.IsFalse(actual);
            }
        }
    }
}
