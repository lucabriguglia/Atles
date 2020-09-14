using System;
using System.Threading;
using System.Threading.Tasks;
using Atles.Data.Caching;
using Atles.Data.Services;
using Atles.Domain.Sites;
using Atles.Domain.Sites.Commands;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Data.Tests.Services
{
    [TestFixture]
    public class SiteServiceTests : TestFixtureBase
    {
        [Test]
        public async Task Should_update_site_and_add_event()
        {
            var options = Shared.CreateContextOptions();
            var site = new Site(Guid.NewGuid(), "Name", "Title");

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Sites.Add(site);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var command = Fixture.Build<UpdateSite>()
                        .With(x => x.SiteId, site.Id)
                        .Create();

                var cacheManager = new Mock<ICacheManager>();

                var updateValidator = new Mock<IValidator<UpdateSite>>();
                updateValidator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var sut = new SiteService(dbContext,
                    cacheManager.Object,
                    updateValidator.Object);

                await sut.UpdateAsync(command);

                var updatedSite = await dbContext.Sites.FirstOrDefaultAsync(x => x.Id == command.SiteId);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.SiteId);

                updateValidator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.AreEqual(command.Title, updatedSite.Title);
                Assert.NotNull(@event);
            }
        }
    }
}
