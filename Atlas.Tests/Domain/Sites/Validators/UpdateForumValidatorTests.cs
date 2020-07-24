using Atlas.Domain.Sites.Commands;
using Atlas.Domain.Sites.Validators;
using AutoFixture;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Atlas.Tests.Domain.Sites.Validators
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
            var command = Fixture.Build<UpdateSite>().With(x => x.Title, new string('*', 101)).Create();

            var sut = new UpdateSiteValidator();

            sut.ShouldHaveValidationErrorFor(x => x.Title, command);
        }
    }
}
