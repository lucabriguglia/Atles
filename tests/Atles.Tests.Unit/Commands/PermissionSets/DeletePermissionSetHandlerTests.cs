using Atles.Commands.Handlers.PermissionSets;
using Atles.Commands.PermissionSets;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Commands.PermissionSets;

[TestFixture]
public class DeletePermissionSetHandlerTests : TestFixtureBase
{
    [Test]
    public async Task Should_delete_permission_set_and_reorder_other_categories_and_add_event()
    {
        var options = Shared.CreateContextOptions();

        var siteId = Guid.NewGuid();

        var permissionSet = new PermissionSet(siteId, "Permission Set", new List<Permission>());

        await using (var dbContext = new AtlesDbContext(options))
        {
            dbContext.PermissionSets.Add(permissionSet);
            await dbContext.SaveChangesAsync();
        }

        await using (var dbContext = new AtlesDbContext(options))
        {
            var command = Fixture.Build<DeletePermissionSet>()
                .With(x => x.PermissionSetId, permissionSet.Id)
                .With(x => x.SiteId, siteId)
                .Create();

            var cacheManager = new Mock<ICacheManager>();

            var sut = new DeletePermissionSetHandler(dbContext, cacheManager.Object);

            await sut.Handle(command);

            var permissionSetDeleted = await dbContext.PermissionSets.FirstOrDefaultAsync(x => x.Id == command.PermissionSetId);
            var permissionSetEvent = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.PermissionSetId);

            Assert.AreEqual(PermissionSetStatusType.Deleted, permissionSetDeleted.Status);
            Assert.NotNull(permissionSetEvent);
        }
    }
}
