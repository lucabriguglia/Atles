using Atlas.Domain.Sites.Commands;
using Atlas.Domain.Sites.Validators;
using AutoFixture;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Atlas.Domain.Tests.Sites.Validators
{
    [TestFixture]
    public class UpdateSiteValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_title_is_empty()
        {
            var command = Fixture.Build<UpdateSite>().With(x => x.Title, string.Empty).Create();

            var sut = new UpdateSiteValidator();

            sut.ShouldHaveValidationErrorFor(x => x.Title, command);
        }

        [Test]
        public void Should_have_validation_error_when_title_is_too_long()
        {
            var command = Fixture.Build<UpdateSite>().With(x => x.Title, new string('*', 51)).Create();

            var sut = new UpdateSiteValidator();

            sut.ShouldHaveValidationErrorFor(x => x.Title, command);
        }

        [Test]
        public void Should_have_validation_error_when_theme_is_empty()
        {
            var command = Fixture.Build<UpdateSite>().With(x => x.Theme, string.Empty).Create();

            var sut = new UpdateSiteValidator();

            sut.ShouldHaveValidationErrorFor(x => x.Theme, command);
        }

        [Test]
        public void Should_have_validation_error_when_theme_is_too_long()
        {
            var command = Fixture.Build<UpdateSite>().With(x => x.Theme, new string('*', 251)).Create();

            var sut = new UpdateSiteValidator();

            sut.ShouldHaveValidationErrorFor(x => x.Theme, command);
        }

        [Test]
        public void Should_have_validation_error_when_css_is_empty()
        {
            var command = Fixture.Build<UpdateSite>().With(x => x.Css, string.Empty).Create();

            var sut = new UpdateSiteValidator();

            sut.ShouldHaveValidationErrorFor(x => x.Css, command);
        }

        [Test]
        public void Should_have_validation_error_when_css_is_too_long()
        {
            var command = Fixture.Build<UpdateSite>().With(x => x.Css, new string('*', 251)).Create();

            var sut = new UpdateSiteValidator();

            sut.ShouldHaveValidationErrorFor(x => x.Css, command);
        }
    }
}
