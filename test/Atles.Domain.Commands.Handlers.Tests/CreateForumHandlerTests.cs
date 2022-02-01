using Atles.Data;
using Atles.Data.Caching;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Domain.Commands.Handlers.Tests
{
    [TestFixture]
    public class CreateForumHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_create_new_forum_and_add_event()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var command = Fixture.Create<CreateForum>();

                var cacheManager = new Mock<ICacheManager>();

                var validator = new Mock<IValidator<CreateForum>>();
                validator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var sut = new CreateForumHandler(dbContext,
                    cacheManager.Object, 
                    validator.Object);

                await sut.Handle(command);

                var forum = await dbContext.Forums.FirstOrDefaultAsync(x => x.Id == command.ForumId);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.ForumId);

                validator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.NotNull(forum);
                Assert.AreEqual(1, forum.SortOrder);
                Assert.NotNull(@event);
            }
        }
    }
}
