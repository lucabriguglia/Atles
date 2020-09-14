using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Atlas.Data.Caching;
using Atlas.Data.Services;
using Atlas.Domain;
using Atlas.Domain.PermissionSets;
using Atlas.Domain.PermissionSets.Commands;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atlas.Data.Tests.Services
{
    [TestFixture]
    public class PermissionSetServiceTests : TestFixtureBase
    {
        [Test]
        public async Task Should_create_new_permission_set_and_add_event()
        {
            using (var dbContext = new AtlasDbContext(Shared.CreateContextOptions()))
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

                var createValidator = new Mock<IValidator<CreatePermissionSet>>();
                createValidator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var updateValidator = new Mock<IValidator<UpdatePermissionSet>>();

                var sut = new PermissionSetService(dbContext,
                    cacheManager.Object, 
                    createValidator.Object, 
                    updateValidator.Object);

                await sut.CreateAsync(command);

                var permissionSet = await dbContext.PermissionSets.FirstOrDefaultAsync(x => x.Id == command.Id);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                createValidator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.NotNull(permissionSet);
                Assert.AreEqual(command.Name, permissionSet.Name);
                Assert.NotNull(@event);
            }
        }

        [Test]
        public async Task Should_update_permission_set_and_add_event()
        {
            var options = Shared.CreateContextOptions();
            var permissionSet = new PermissionSet(Guid.NewGuid(), Guid.NewGuid(), "Default", new List<PermissionCommand>());

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.PermissionSets.Add(permissionSet);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var command = new UpdatePermissionSet
                {
                    SiteId = permissionSet.SiteId,
                    Id = permissionSet.Id,
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

                var createValidator = new Mock<IValidator<CreatePermissionSet>>();

                var updateValidator = new Mock<IValidator<UpdatePermissionSet>>();
                updateValidator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var sut = new PermissionSetService(dbContext,
                    cacheManager.Object,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.UpdateAsync(command);

                var updatedPermissionSet = await dbContext.PermissionSets.FirstOrDefaultAsync(x => x.Id == command.Id);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                updateValidator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.AreEqual(command.Name, updatedPermissionSet.Name);
                Assert.NotNull(@event);
            }
        }

        [Test]
        public async Task Should_delete_permission_set_and_reorder_other_categories_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var siteId = Guid.NewGuid();

            var permissionSet = new PermissionSet(siteId, "Permission Set", new List<PermissionCommand>());

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.PermissionSets.Add(permissionSet);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var command = Fixture.Build<DeletePermissionSet>()
                        .With(x => x.Id, permissionSet.Id)
                        .With(x => x.SiteId, siteId)
                    .Create();

                var cacheManager = new Mock<ICacheManager>();
                var createValidator = new Mock<IValidator<CreatePermissionSet>>();
                var updateValidator = new Mock<IValidator<UpdatePermissionSet>>();

                var sut = new PermissionSetService(dbContext,
                    cacheManager.Object,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.DeleteAsync(command);

                var permissionSetDeleted = await dbContext.PermissionSets.FirstOrDefaultAsync(x => x.Id == command.Id);
                var permissionSetEvent = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                Assert.AreEqual(PermissionSetStatusType.Deleted, permissionSetDeleted.Status);
                Assert.NotNull(permissionSetEvent);
            }
        }
    }
}
