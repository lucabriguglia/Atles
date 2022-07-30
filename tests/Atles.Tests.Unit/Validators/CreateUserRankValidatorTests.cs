using Atles.Commands.Handlers.UserRanks.Validators;
using Atles.Commands.UserRanks;
using Atles.Core;
using Atles.Domain.Rules.UserRanks;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Validators
{
    [Ignore("WIP")]
    [TestFixture]
    public class CreateUserRankValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_name_is_empty()
        {
            var command = Fixture.Build<CreateUserRank>().With(x => x.Name, string.Empty).Create();

            var dispatcher = new Mock<IDispatcher>();

            var sut = new CreateUserRankValidator(dispatcher.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_too_long()
        {
            var command = Fixture.Build<CreateUserRank>().With(x => x.Name, new string('*', 51)).Create();

            var dispatcher = new Mock<IDispatcher>();

            var sut = new CreateUserRankValidator(dispatcher.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_not_unique()
        {
            var command = Fixture.Create<CreateUserRank>();

            var dispatcher = new Mock<IDispatcher>();
            dispatcher.Setup(x => x.Get(It.IsAny<IsUserRankNameUnique>())).ReturnsAsync(false);

            var sut = new CreateUserRankValidator(dispatcher.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_not_have_validation_error_when_description_is_empty()
        {
            var command = Fixture.Build<CreateUserRank>().With(x => x.Description, string.Empty).Create();

            var dispatcher = new Mock<IDispatcher>();

            var sut = new CreateUserRankValidator(dispatcher.Object);

            sut.ShouldNotHaveValidationErrorFor(x => x.Description, command);
        }

        [Test]
        public void Should_have_validation_error_when_description_is_too_long()
        {
            var command = Fixture.Build<CreateUserRank>().With(x => x.Description, new string('*', 51)).Create();

            var dispatcher = new Mock<IDispatcher>();

            var sut = new CreateUserRankValidator(dispatcher.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Description, command);
        }
    }
}
