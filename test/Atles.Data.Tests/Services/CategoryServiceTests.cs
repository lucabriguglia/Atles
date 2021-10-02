using System;
using System.Threading.Tasks;
using Atles.Data.Caching;
using Atles.Data.Services;
using Atles.Domain;
using Atles.Domain.Categories;
using Atles.Domain.Categories.Commands;
using Atles.Domain.Forums;
using AutoFixture;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Data.Tests.Services
{
    [TestFixture]
    public class CategoryServiceTests : TestFixtureBase
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

                var sut = new CategoryService(dbContext,
                    cacheManager.Object,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.MoveAsync(command);

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

                var sut = new CategoryService(dbContext,
                    cacheManager.Object,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.MoveAsync(command);

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
        public async Task Should_delete_category_and_reorder_other_categories_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var siteId = Guid.NewGuid();

            var category1 = new Category(siteId, "Category 1", 1, Guid.NewGuid());
            var category2 = new Category(siteId, "Category 2", 2, Guid.NewGuid());
            var category3 = new Category(siteId, "Category 3", 3, Guid.NewGuid());
            var category4 = new Category(siteId, "Category 4", 4, Guid.NewGuid());

            var forum1 = new Forum(category2.Id, "Forum 1", "my-forum-1", "My Forum", 1);
            var forum2 = new Forum(category2.Id, "Forum 2", "my-forum-2", "My Forum", 2);

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Categories.Add(category1);
                dbContext.Categories.Add(category2);
                dbContext.Categories.Add(category3);
                dbContext.Categories.Add(category4);

                dbContext.Forums.Add(forum1);
                dbContext.Forums.Add(forum2);

                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var command = Fixture.Build<DeleteCategory>()
                        .With(x => x.Id, category2.Id)
                        .With(x => x.SiteId, siteId)
                    .Create();

                var cacheManager = new Mock<ICacheManager>();
                var createValidator = new Mock<IValidator<CreateCategory>>();
                var updateValidator = new Mock<IValidator<UpdateCategory>>();

                var sut = new CategoryService(dbContext,
                    cacheManager.Object,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.DeleteAsync(command);

                var category1Reordered = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == category1.Id);
                var category2Deleted = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == command.Id);
                var category3Reordered = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == category3.Id);
                var category4Reordered = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == category4.Id);

                var forum1Deleted = await dbContext.Forums.FirstOrDefaultAsync(x => x.Id == forum1.Id);
                var forum2Deleted = await dbContext.Forums.FirstOrDefaultAsync(x => x.Id == forum2.Id);

                var category1Event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == category1.Id);
                var category2Event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);
                var category3Event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == category3.Id);
                var category4Event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == category4.Id);

                var forum1Event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == forum1.Id);
                var forum2Event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == forum2.Id);

                Assert.AreEqual(category1.SortOrder, category1Reordered.SortOrder);
                Assert.AreEqual(CategoryStatusType.Deleted, category2Deleted.Status);
                Assert.AreEqual(category2.SortOrder, category3Reordered.SortOrder);
                Assert.AreEqual(category3.SortOrder, category4Reordered.SortOrder);
                Assert.AreEqual(ForumStatusType.Deleted, forum1Deleted.Status);
                Assert.AreEqual(ForumStatusType.Deleted, forum2Deleted.Status);
                Assert.NotNull(category1Event);
                Assert.NotNull(category2Event);
                Assert.NotNull(category3Event);
                Assert.NotNull(category4Event);
                Assert.NotNull(forum1Event);
                Assert.NotNull(forum2Event);
            }
        }
    }
}
