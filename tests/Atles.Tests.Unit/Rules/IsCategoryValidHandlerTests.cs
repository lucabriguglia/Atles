using Atles.Data;
using Atles.Domain;
using Atles.Domain.Rules.Categories;
using Atles.Domain.Rules.Handlers.Categories;
using AutoFixture;
using NUnit.Framework;

namespace Atles.Tests.Unit.Rules
{
    [TestFixture]
    public class IsCategoryValidHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_return_false_when_category_is_not_valid()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var sut = new IsCategoryValidHandler(dbContext);
                var query = Fixture.Create<IsCategoryValid>();
                var actual = await sut.Handle(query);

                Assert.IsFalse(actual.AsT0);
            }
        }

        [Test]
        public async Task Should_return_true_when_category_is_valid()
        {
            var options = Shared.CreateContextOptions();

            var siteId = Guid.NewGuid();
            var category = new Category(siteId, "Category Name", 1, Guid.NewGuid());

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Categories.Add(category);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var sut = new IsCategoryValidHandler(dbContext);
                var query = new IsCategoryValid { SiteId = siteId, Id = category.Id };
                var actual = await sut.Handle(query);

                Assert.IsTrue(actual.AsT0);
            }
        }
    }
}
