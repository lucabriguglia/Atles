using System;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain.Categories;
using Atles.Domain.Categories.Commands;
using Atles.Domain.Handlers.Categories.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Domain.Handlers.Tests.Categories.Commands
{
    [TestFixture]
    public class MoveCategoryHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_move_category_up_and_add_events()
        {
            var options = Shared.CreateContextOptions();

            var siteId = Guid.NewGuid();

            var category1 = new Category(siteId, "Category 1", 1, Guid.NewGuid());
            var category2 = new Category(siteId, "Category 2", 2, Guid.NewGuid());

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Categories.Add(category1);
                dbContext.Categories.Add(category2);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var command = new MoveCategory
                {
                    Id = category2.Id,
                    Direction = Direction.Up,
                    SiteId = siteId
                };

                var cacheManager = new Mock<ICacheManager>();
                var createValidator = new Mock<IValidator<CreateCategory>>();
                var updateValidator = new Mock<IValidator<UpdateCategory>>();

                var sut = new MoveCategoryHandler(dbContext, cacheManager.Object);

                await sut.Handle(command);

                var updatedCategory1 = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == category1.Id);
                var updatedCategory2 = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == category2.Id);

                var event1 = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == category1.Id);
                var event2 = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == category2.Id);

                Assert.AreEqual(category2.SortOrder, updatedCategory1.SortOrder);
                Assert.AreEqual(category1.SortOrder, updatedCategory2.SortOrder);
                Assert.NotNull(event1);
                Assert.NotNull(event2);
            }
        }

        [Test]
        public async Task Should_move_category_down_and_add_events()
        {
            var options = Shared.CreateContextOptions();

            var siteId = Guid.NewGuid();

            var category1 = new Category(siteId, "Category 1", 1, Guid.NewGuid());
            var category2 = new Category(siteId, "Category 2", 2, Guid.NewGuid());

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Categories.Add(category1);
                dbContext.Categories.Add(category2);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var command = new MoveCategory
                {
                    Id = category1.Id,
                    Direction = Direction.Down,
                    SiteId = siteId
                };

                var cacheManager = new Mock<ICacheManager>();
                var createValidator = new Mock<IValidator<CreateCategory>>();
                var updateValidator = new Mock<IValidator<UpdateCategory>>();

                var sut = new MoveCategoryHandler(dbContext, cacheManager.Object);

                await sut.Handle(command);

                var updatedCategory1 = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == category1.Id);
                var updatedCategory2 = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == category2.Id);

                var event1 = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == category1.Id);
                var event2 = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == category2.Id);

                Assert.AreEqual(category2.SortOrder, updatedCategory1.SortOrder);
                Assert.AreEqual(category1.SortOrder, updatedCategory2.SortOrder);
                Assert.NotNull(event1);
                Assert.NotNull(event2);
            }
        }
    }
}
