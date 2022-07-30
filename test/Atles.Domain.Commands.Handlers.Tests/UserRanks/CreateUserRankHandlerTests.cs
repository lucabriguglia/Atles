using Atles.Commands.UserRanks;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain.Commands.Handlers.UserRanks;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Domain.Commands.Handlers.Tests.UserRanks
{
    [TestFixture]
    public class CreateUserRankHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_create_new_userRank_and_add_event()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var command = Fixture.Create<CreateUserRank>();

                var cacheManager = new Mock<ICacheManager>();

                var validator = new Mock<IValidator<CreateUserRank>>();
                validator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var sut = new CreateUserRankHandler(dbContext, validator.Object, cacheManager.Object);

                await sut.Handle(command);

                var userRank = await dbContext.UserRanks.FirstOrDefaultAsync();
                var @event = await dbContext.Events.FirstOrDefaultAsync();

                validator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.NotNull(userRank);
                Assert.AreEqual(1, userRank.SortOrder);
                Assert.NotNull(@event);
            }
        }
    }
}
