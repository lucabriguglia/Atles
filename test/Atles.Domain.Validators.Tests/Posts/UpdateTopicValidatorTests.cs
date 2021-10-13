using Atles.Domain.Posts.Commands;
using Atles.Domain.Validators.Posts;
using AutoFixture;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Atles.Domain.Tests.Posts.Validators
{
    [TestFixture]
    public class UpdateTopicValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_title_is_empty()
        {
            var command = Fixture.Build<UpdateTopic>().With(x => x.Title, string.Empty).Create();

            var sut = new UpdateTopicValidator();

            sut.ShouldHaveValidationErrorFor(x => x.Title, command);
        }

        [Test]
        public void Should_have_validation_error_when_title_is_too_long()
        {
            var command = Fixture.Build<UpdateTopic>().With(x => x.Title, new string('*', 101)).Create();

            var sut = new UpdateTopicValidator();

            sut.ShouldHaveValidationErrorFor(x => x.Title, command);
        }

        [Test]
        public void Should_have_validation_error_when_slug_is_too_long()
        {
            var command = Fixture.Build<UpdateTopic>().With(x => x.Slug, new string('*', 51)).Create();

            var sut = new UpdateTopicValidator();

            sut.ShouldHaveValidationErrorFor(x => x.Slug, command);
        }

        [Test]
        public void Should_have_validation_error_when_content_is_empty()
        {
            var command = Fixture.Build<UpdateTopic>().With(x => x.Content, string.Empty).Create();

            var sut = new UpdateTopicValidator();

            sut.ShouldHaveValidationErrorFor(x => x.Content, command);
        }
    }
}
