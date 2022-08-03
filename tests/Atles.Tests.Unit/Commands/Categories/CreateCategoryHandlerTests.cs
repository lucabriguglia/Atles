using Atles.Commands.Categories;
using Atles.Commands.Handlers.Categories;
using Atles.Data;
using Atles.Data.Caching;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Commands.Categories;

[TestFixture]
public class CreateCategoryHandlerTests : TestFixtureBase
{
    [Test]
    public async Task Should_create_new_category_and_add_event()
    {
        await using var dbContext = new AtlesDbContext(Shared.CreateContextOptions());

        var command = Fixture.Create<CreateCategory>();

        var cacheManager = new Mock<ICacheManager>();

        var sut = new CreateCategoryHandler(dbContext, cacheManager.Object);

        await sut.Handle(command);

        var category = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == command.CategoryId);
        var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.CategoryId);

        Assert.NotNull(category);
        Assert.AreEqual(1, category.SortOrder);
        Assert.NotNull(@event);
    }
}
