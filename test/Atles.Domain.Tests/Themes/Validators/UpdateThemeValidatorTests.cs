using Atles.Domain.Themes;
using Atles.Domain.Themes.Commands;
using Atles.Domain.Themes.Validators;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Atles.Domain.Tests.Themes.Validators
{
    [TestFixture]
    public class UpdateThemeValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_name_is_empty()
        {
            var command = Fixture.Build<UpdateTheme>().With(x => x.Name, string.Empty).Create();

            var rules = new Mock<IThemeRules>();

            var sut = new UpdateThemeValidator(rules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_too_long()
        {
            var command = Fixture.Build<UpdateTheme>().With(x => x.Name, new string('*', 51)).Create();

            var rules = new Mock<IThemeRules>();

            var sut = new UpdateThemeValidator(rules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_not_unique()
        {
            var command = Fixture.Create<UpdateTheme>();

            var rules = new Mock<IThemeRules>();
            rules.Setup(x => x.IsNameUniqueAsync(command.SiteId, command.Name, command.Id)).ReturnsAsync(false);

            var sut = new UpdateThemeValidator(rules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }
    }
}