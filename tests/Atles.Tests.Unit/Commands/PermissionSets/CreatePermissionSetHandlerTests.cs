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
public class CreatePermissionSetHandlerTests : TestFixtureBase
{
    [Test]
    public async Task Should_create_new_permission_set_and_add_event()
    {
        await using var dbContext = new AtlesDbContext(Shared.CreateContextOptions());
        var command = new CreatePermissionSet
        {
            Name = "Permission Set",
            Permissions = new List<PermissionCommand>
            {
                new PermissionCommand
                {
                    Type = PermissionType.Start,
                    RoleId = Guid.NewGuid().ToString()
                }
            },
            SiteId = Guid.NewGuid()
        };

        var cacheManager = new Mock<ICacheManager>();

        var sut = new CreatePermissionSetHandler(dbContext);

        await sut.Handle(command);

        var permissionSet = await dbContext.PermissionSets.FirstOrDefaultAsync(x => x.Id == command.PermissionSetId);
        var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.PermissionSetId);

        Assert.NotNull(permissionSet);
        Assert.AreEqual(command.Name, permissionSet.Name);
        Assert.NotNull(@event);
    }
}
