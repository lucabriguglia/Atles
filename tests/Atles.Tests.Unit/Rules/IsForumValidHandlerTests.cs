﻿using Atles.Data;
using Atles.Domain;
using Atles.Domain.Rules.Forums;
using Atles.Domain.Rules.Handlers.Forums;
using NUnit.Framework;

namespace Atles.Tests.Unit.Rules
{
    [TestFixture]
    public class IsForumValidHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_return_true_when_forum_is_valid()
        {
            var options = Shared.CreateContextOptions();
            var category = new Category(Guid.NewGuid(), Guid.NewGuid(), "Category", 1, Guid.NewGuid());
            var forum = new Forum(Guid.NewGuid(), category.Id, "Forum", "my-forum", "My Forum", 1);

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var sut = new IsForumValidHandler(dbContext);
                var query = new IsForumValid
                {
                    SiteId = forum.Category.SiteId,
                    Id = forum.Id
                };
                var actual = await sut.Handle(query);

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_false_when_forum_is_not_valid()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var sut = new IsForumValidHandler(dbContext);
                var query = new IsForumValid
                {
                    SiteId = Guid.NewGuid(),
                    Id = Guid.NewGuid()
                };
                var actual = await sut.Handle(query);

                Assert.IsFalse(actual);
            }
        }
    }
}
