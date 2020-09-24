using System;
using System.Threading;
using System.Threading.Tasks;
using Atles.Data.Caching;
using Atles.Data.Services;
using Atles.Domain.Themes;
using Atles.Domain.Themes.Commands;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Data.Tests.Services
{
    [TestFixture]
    public class ThemeServiceTests : TestFixtureBase
    {
        [Test]
        public async Task Should_create_new_entity_and_add_event()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var command = new CreateTheme
                {
                    Name = "Theme"
                };

                var cacheManager = new Mock<ICacheManager>();

                var createValidator = new Mock<IValidator<CreateTheme>>();
                createValidator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var updateValidator = new Mock<IValidator<UpdateTheme>>();

                var sut = new ThemeService(dbContext,
                    cacheManager.Object,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.CreateAsync(command);

                var entity = await dbContext.Themes.FirstOrDefaultAsync(x => x.Id == command.Id);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                createValidator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.NotNull(entity);
                Assert.AreEqual(command.Name, entity.Name);
                Assert.NotNull(@event);
            }
        }

        [Test]
        public async Task Should_update_entity_and_add_event()
        {
            var options = Shared.CreateContextOptions();
            var entity = Theme.CreateNew(Guid.NewGuid(), Guid.NewGuid(), "Theme");

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Themes.Add(entity);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var command = new UpdateTheme
                {
                    SiteId = entity.SiteId,
                    Id = entity.Id,
                    Name = "Theme Updated"
                };

                var cacheManager = new Mock<ICacheManager>();

                var createValidator = new Mock<IValidator<CreateTheme>>();

                var updateValidator = new Mock<IValidator<UpdateTheme>>();
                updateValidator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var sut = new ThemeService(dbContext,
                    cacheManager.Object,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.UpdateAsync(command);

                var entityUpdated = await dbContext.Themes.FirstOrDefaultAsync(x => x.Id == command.Id);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                updateValidator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.AreEqual(command.Name, entityUpdated.Name);
                Assert.NotNull(@event);
            }
        }

        [Test]
        public async Task Should_delete_entity_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var siteId = Guid.NewGuid();

            var entity = Theme.CreateNew(siteId, "Theme");

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Themes.Add(entity);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var command = Fixture.Build<DeleteTheme>()
                    .With(x => x.Id, entity.Id)
                    .With(x => x.SiteId, siteId)
                    .Create();

                var cacheManager = new Mock<ICacheManager>();
                var createValidator = new Mock<IValidator<CreateTheme>>();
                var updateValidator = new Mock<IValidator<UpdateTheme>>();

                var sut = new ThemeService(dbContext,
                    cacheManager.Object,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.DeleteAsync(command);

                var entityDeleted = await dbContext.Themes.FirstOrDefaultAsync(x => x.Id == command.Id);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                Assert.AreEqual(ThemeStatus.Deleted, entityDeleted.Status);
                Assert.NotNull(@event);
            }
        }
    }
}