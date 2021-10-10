using Atles.Domain.Categories.Rules;
using Atles.Domain.Users.Commands;
using Atles.Domain.Users.Validators;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using OpenCqrs;

namespace Atles.Domain.Tests.Users.Validators
{
    [TestFixture]
    public class UpdateUserValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_display_name_is_empty()
        {
            var command = Fixture.Build<UpdateUser>().With(x => x.DisplayName, string.Empty).Create();

            var sender = new Mock<ISender>();

            var sut = new UpdateUserValidator(sender.Object);

            sut.ShouldHaveValidationErrorFor(x => x.DisplayName, command);
        }

        [Test]
        public void Should_have_validation_error_when_display_name_is_too_long()
        {
            var command = Fixture.Build<UpdateUser>().With(x => x.DisplayName, new string('*', 51)).Create();

            var sender = new Mock<ISender>();

            var sut = new UpdateUserValidator(sender.Object);

            sut.ShouldHaveValidationErrorFor(x => x.DisplayName, command);
        }

        [Test]
        public void Should_have_validation_error_when_display_name_is_not_unique()
        {
            var command = Fixture.Create<UpdateUser>();

            var sender = new Mock<ISender>();
            sender.Setup(x => x.Send(new IsUserDisplayNameUnique { DisplayName = command.DisplayName, Id = command.Id })).ReturnsAsync(false);

            var sut = new UpdateUserValidator(sender.Object);

            sut.ShouldHaveValidationErrorFor(x => x.DisplayName, command);
        }
    }
}
