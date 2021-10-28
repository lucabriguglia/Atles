using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain.Handlers.PermissionSets.Rules;
using Atles.Domain.Models.PermissionSets;
using Atles.Domain.Models.PermissionSets.Commands;
using Atles.Domain.Models.PermissionSets.Rules;
using Atles.Domain.Models.Sites;
using NUnit.Framework;

namespace Atles.Domain.Handlers.Tests.PermissionSets.Rules
{
    [TestFixture]
    public class IsPermissionSetValidHandlerTests : TestFixtureBase
    {
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
                var sut = new IsPermissionSetValidHandler(dbContext);
                var query = new IsPermissionSetValid { SiteId = site.Id, Id = permissionSet.Id };
                var actual = await sut.Handle(query);

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_false_when_permission_set_is_not_valid()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var sut = new IsPermissionSetValidHandler(dbContext);
                var query = new IsPermissionSetValid { SiteId = Guid.NewGuid(), Id = Guid.NewGuid() };
                var actual = await sut.Handle(query);

                Assert.IsFalse(actual);
            }
        }
    }
}
