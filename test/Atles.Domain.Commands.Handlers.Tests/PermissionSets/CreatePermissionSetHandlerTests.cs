using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain.Commands.Handlers.PermissionSets;
using Atles.Domain.Commands.PermissionSets;
using Atles.Domain.Models;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Domain.Commands.Handlers.Tests.PermissionSets
{
    [TestFixture]
    public class CreatePermissionSetHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_create_new_permission_set_and_add_event()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
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

                var validator = new Mock<IValidator<CreatePermissionSet>>();
                validator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var sut = new CreatePermissionSetHandler(dbContext, validator.Object);

                await sut.Handle(command);

                var permissionSet = await dbContext.PermissionSets.FirstOrDefaultAsync(x => x.Id == command.PermissionSetId);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.PermissionSetId);

                validator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.NotNull(permissionSet);
                Assert.AreEqual(command.Name, permissionSet.Name);
                Assert.NotNull(@event);
            }
        }
    }
}
