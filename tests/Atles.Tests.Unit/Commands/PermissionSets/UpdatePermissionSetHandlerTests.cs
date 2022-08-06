using Atles.Commands.Handlers.PermissionSets;
using Atles.Commands.PermissionSets;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Commands.PermissionSets;

[TestFixture]
public class UpdatePermissionSetHandlerTests : TestFixtureBase
{
    [Test]
    public async Task Should_update_permission_set_and_add_event()
    {
        var options = Shared.CreateContextOptions();
        var permissionSet = new PermissionSet(Guid.NewGuid(), Guid.NewGuid(), "Default", new List<Permission>());

        await using (var dbContext = new AtlesDbContext(options))
        {
            dbContext.PermissionSets.Add(permissionSet);
            await dbContext.SaveChangesAsync();
        }

        await using (var dbContext = new AtlesDbContext(options))
        {
            var command = new UpdatePermissionSet
            {
                SiteId = permissionSet.SiteId,
                PermissionSetId = permissionSet.Id,
                Name = "Permission Set",
                Permissions = new List<PermissionCommand>
                {
                    new PermissionCommand
                    {
                        Type = PermissionType.Start,
                        RoleId = Guid.NewGuid().ToString()
                    }
                }
            };

            var cacheManager = new Mock<ICacheManager>();

            var sut = new UpdatePermissionSetHandler(dbContext, cacheManager.Object);

            await sut.Handle(command);

            var updatedPermissionSet = await dbContext.PermissionSets.FirstOrDefaultAsync(x => x.Id == command.PermissionSetId);
            var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.PermissionSetId);

            Assert.AreEqual(command.Name, updatedPermissionSet.Name);
            Assert.NotNull(@event);
        }
    }
}
