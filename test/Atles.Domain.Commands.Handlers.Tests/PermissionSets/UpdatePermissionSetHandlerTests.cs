using Atles.Commands.Handlers.PermissionSets;
using Atles.Commands.PermissionSets;
using Atles.Data;
using Atles.Data.Caching;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Domain.Commands.Handlers.Tests.PermissionSets
{
    [TestFixture]
    public class UpdatePermissionSetHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_update_permission_set_and_add_event()
        {
            var options = Shared.CreateContextOptions();
            var permissionSet = new PermissionSet(Guid.NewGuid(), Guid.NewGuid(), "Default", new List<Permission>());

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.PermissionSets.Add(permissionSet);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
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

                var validator = new Mock<IValidator<UpdatePermissionSet>>();
                validator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var sut = new UpdatePermissionSetHandler(dbContext,
                    validator.Object,
                    cacheManager.Object);

                await sut.Handle(command);

                var updatedPermissionSet = await dbContext.PermissionSets.FirstOrDefaultAsync(x => x.Id == command.PermissionSetId);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.PermissionSetId);

                validator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.AreEqual(command.Name, updatedPermissionSet.Name);
                Assert.NotNull(@event);
            }
        }
    }
}
