using Atles.Commands.Categories;
using Atles.Commands.Handlers.Categories;
using Atles.Data;
using Atles.Data.Caching;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Commands.Categories
{
    [TestFixture]
    public class CreateCategoryHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_create_new_category_and_add_event()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var command = Fixture.Create<CreateCategory>();

                var cacheManager = new Mock<ICacheManager>();

                var validator = new Mock<IValidator<CreateCategory>>();
                validator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var sut = new CreateCategoryHandler(dbContext, validator.Object, cacheManager.Object);

                await sut.Handle(command);

                var category = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == command.CategoryId);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.CategoryId);

                validator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.NotNull(category);
                Assert.AreEqual(1, category.SortOrder);
                Assert.NotNull(@event);
            }
        }
    }
}
