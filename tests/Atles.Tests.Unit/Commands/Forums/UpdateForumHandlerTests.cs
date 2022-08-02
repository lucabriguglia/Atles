using Atles.Commands.Forums;
using Atles.Commands.Handlers.Forums;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Commands.Forums
{
    [TestFixture]
    public class UpdateForumHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_update_forum_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var categoryId = Guid.NewGuid();
            var siteId = Guid.NewGuid();

            var category = new Category(categoryId, siteId, "Category", 1, Guid.NewGuid());
            var forum = new Forum(categoryId, "Forum 1", "my-forum", "Forum 1", 1);

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Categories.Add(category);
                dbContext.Forums.Add(forum);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var command = Fixture.Build<UpdateForum>()
                        .With(x => x.ForumId, forum.Id)
                        .With(x => x.CategoryId, forum.CategoryId)
                        .With(x => x.SiteId, siteId)
                    .Create();

                var cacheManager = new Mock<ICacheManager>();

                var validator = new Mock<IValidator<UpdateForum>>();
                validator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var sut = new UpdateForumHandler(dbContext,
                    cacheManager.Object,
                    validator.Object);

                await sut.Handle(command);

                var updatedForum = await dbContext.Forums.FirstOrDefaultAsync(x => x.Id == command.ForumId);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.ForumId);

                validator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.AreEqual(command.Name, updatedForum.Name);
                Assert.NotNull(@event);
            }
        }
    }
}
