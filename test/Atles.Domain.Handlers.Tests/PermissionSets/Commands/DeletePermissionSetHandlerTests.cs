using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain.Handlers.PermissionSets.Commands;
using Atles.Domain.Models.PermissionSets;
using Atles.Domain.Models.PermissionSets.Commands;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Domain.Handlers.Tests.PermissionSets.Commands
{
    [TestFixture]
    public class DeletePermissionSetHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_delete_permission_set_and_reorder_other_categories_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var siteId = Guid.NewGuid();

            var permissionSet = new PermissionSet(siteId, "Permission Set", new List<PermissionCommand>());

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.PermissionSets.Add(permissionSet);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var command = Fixture.Build<DeletePermissionSet>()
                        .With(x => x.Id, permissionSet.Id)
                        .With(x => x.SiteId, siteId)
                    .Create();

                var cacheManager = new Mock<ICacheManager>();

                var sut = new DeletePermissionSetHandler(dbContext, cacheManager.Object);

                await sut.Handle(command);

                var permissionSetDeleted = await dbContext.PermissionSets.FirstOrDefaultAsync(x => x.Id == command.Id);
                var permissionSetEvent = await dbContext.HistoryItems.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                Assert.AreEqual(PermissionSetStatusType.Deleted, permissionSetDeleted.Status);
                Assert.NotNull(permissionSetEvent);
            }
        }
    }
}
