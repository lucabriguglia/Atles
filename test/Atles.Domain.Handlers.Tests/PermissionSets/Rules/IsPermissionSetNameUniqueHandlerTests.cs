using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain.Handlers.PermissionSets.Rules;
using Atles.Domain.Models;
using Atles.Domain.Rules;
using NUnit.Framework;

namespace Atles.Domain.Handlers.Tests.PermissionSets.Rules
{
    [TestFixture]
    public class IsPermissionSetNameUniqueHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_return_true_when_name_is_unique()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var sut = new IsPermissionSetNameUniqueHandler(dbContext);
                var query = new IsPermissionSetNameUnique { SiteId = Guid.NewGuid(), Name = "My Permission Set" };
                var actual = await sut.Handle(query);

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_true_when_name_is_unique_for_existing_permission_set()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var sut = new IsPermissionSetNameUniqueHandler(dbContext);
                var query = new IsPermissionSetNameUnique { SiteId = Guid.NewGuid(), Name = "My Permission Set", Id = Guid.NewGuid() };
                var actual = await sut.Handle(query);

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
                var permissionSet = new PermissionSet(siteId, permissionSetName, new List<Permission>());
                dbContext.Sites.Add(site);
                dbContext.PermissionSets.Add(permissionSet);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var sut = new IsPermissionSetNameUniqueHandler(dbContext);
                var query = new IsPermissionSetNameUnique { SiteId = siteId, Name = permissionSetName };
                var actual = await sut.Handle(query);

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
                var permissionSet1 = new PermissionSet(siteId, "Permission Set 1", new List<Permission>());
                var permissionSet2 = new PermissionSet(permissionSetId, siteId, "Permission Set 2", new List<Permission>());
                dbContext.Sites.Add(site);
                dbContext.PermissionSets.Add(permissionSet1);
                dbContext.PermissionSets.Add(permissionSet2);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var sut = new IsPermissionSetNameUniqueHandler(dbContext);
                var query = new IsPermissionSetNameUnique { SiteId = siteId, Name = "Permission Set 1", Id = permissionSetId };
                var actual = await sut.Handle(query);

                Assert.IsFalse(actual);
            }
        }
    }
}
