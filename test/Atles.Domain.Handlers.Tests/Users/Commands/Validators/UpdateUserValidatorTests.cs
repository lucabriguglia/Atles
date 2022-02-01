using Atles.Core;
using Atles.Domain.Commands;
using Atles.Domain.Handlers.Users.Commands.Validators;
using Atles.Domain.Rules;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Atles.Domain.Handlers.Tests.Users.Commands.Validators
{
    [TestFixture]
    public class UpdateUserValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_display_name_is_empty()
        {
            var command = Fixture.Build<UpdateUser>().With(x => x.DisplayName, string.Empty).Create();

            var dispatcher = new Mock<IDispatcher>();

            var sut = new UpdateUserValidator(dispatcher.Object);

            sut.ShouldHaveValidationErrorFor(x => x.DisplayName, command);
        }

        [Test]
        public void Should_have_validation_error_when_display_name_is_too_long()
        {
            var command = Fixture.Build<UpdateUser>().With(x => x.DisplayName, new string('*', 51)).Create();

            var dispatcher = new Mock<IDispatcher>();

            var sut = new UpdateUserValidator(dispatcher.Object);

            sut.ShouldHaveValidationErrorFor(x => x.DisplayName, command);
        }

        [Test]
        public void Should_have_validation_error_when_display_name_is_not_unique()
        {
            var command = Fixture.Create<UpdateUser>();

            var dispatcher = new Mock<IDispatcher>();
            dispatcher.Setup(x => x.Get(new IsUserDisplayNameUnique { DisplayName = command.DisplayName, Id = command.Id })).ReturnsAsync(false);

            var sut = new UpdateUserValidator(dispatcher.Object);

            sut.ShouldHaveValidationErrorFor(x => x.DisplayName, command);
        }
    }
}
