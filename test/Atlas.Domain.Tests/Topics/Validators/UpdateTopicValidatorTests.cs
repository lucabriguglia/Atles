using Atlas.Domain.Posts.Commands;
using Atlas.Domain.Posts.Validators;
using AutoFixture;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Atlas.Domain.Tests.Topics.Validators
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
        public void Should_have_validation_error_when_content_is_empty()
        {
            var command = Fixture.Build<UpdateTopic>().With(x => x.Content, string.Empty).Create();

            var sut = new UpdateTopicValidator();

            sut.ShouldHaveValidationErrorFor(x => x.Content, command);
        }
    }
}
