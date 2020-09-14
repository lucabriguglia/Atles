using Atlas.Domain.Users;
using Atlas.Domain.Users.Commands;
using Atlas.Domain.Users.Validators;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Atlas.Domain.Tests.Users.Validators
{
    [TestFixture]
    public class UpdateUserValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_display_name_is_empty()
        {
            var command = Fixture.Build<UpdateUser>().With(x => x.DisplayName, string.Empty).Create();

            var userRules = new Mock<IUserRules>();

            var sut = new UpdateUserValidator(userRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.DisplayName, command);
        }

        [Test]
        public void Should_have_validation_error_when_display_name_is_too_long()
        {
            var command = Fixture.Build<UpdateUser>().With(x => x.DisplayName, new string('*', 51)).Create();

            var userRules = new Mock<IUserRules>();

            var sut = new UpdateUserValidator(userRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.DisplayName, command);
        }

        [Test]
        public void Should_have_validation_error_when_display_name_is_not_unique()
        {
            var command = Fixture.Create<UpdateUser>();

            var userRules = new Mock<IUserRules>();
            userRules.Setup(x => x.IsDisplayNameUniqueAsync(command.DisplayName, command.Id)).ReturnsAsync(false);

            var sut = new UpdateUserValidator(userRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.DisplayName, command);
        }
    }
}
