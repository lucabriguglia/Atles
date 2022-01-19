using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain.Handlers.PermissionSets.Commands;
using Atles.Domain.Models.PermissionSets;
using Atles.Domain.Models.PermissionSets.Commands;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Domain.Handlers.Tests.PermissionSets.Commands
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
                    }
                };

                var cacheManager = new Mock<ICacheManager>();

                var validator = new Mock<IValidator<CreatePermissionSet>>();
                validator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var sut = new CreatePermissionSetHandler(dbContext, validator.Object);

                await sut.Handle(command);

                var permissionSet = await dbContext.PermissionSets.FirstOrDefaultAsync(x => x.Id == command.Id);
                var @event = await dbContext.HistoryItems.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                validator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.NotNull(permissionSet);
                Assert.AreEqual(command.Name, permissionSet.Name);
                Assert.NotNull(@event);
            }
        }
    }
}
