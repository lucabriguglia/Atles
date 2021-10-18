using Atles.Domain.Forums.Rules;
using Atles.Domain.Posts.Commands;
using Atles.Domain.Validators.Posts;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using OpenCqrs;

namespace Atles.Domain.Tests.Posts.Validators
{
    [TestFixture]
    public class CreateTopicValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_title_is_empty()
        {
            var command = Fixture.Build<CreateTopic>().With(x => x.Title, string.Empty).Create();

            var dispatcher = new Mock<IDispatcher>();

            var sut = new CreateTopicValidator(dispatcher.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Title, command);
        }

        [Test]
        public void Should_have_validation_error_when_title_is_too_long()
        {
            var command = Fixture.Build<CreateTopic>().With(x => x.Title, new string('*', 101)).Create();

            var dispatcher = new Mock<IDispatcher>();

            var sut = new CreateTopicValidator(dispatcher.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Title, command);
        }

        [Test]
        public void Should_have_validation_error_when_content_is_empty()
        {
            var command = Fixture.Build<CreateTopic>().With(x => x.Content, string.Empty).Create();

            var dispatcher = new Mock<IDispatcher>();

            var sut = new CreateTopicValidator(dispatcher.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Content, command);
        }

        [Test]
        public void Should_have_validation_error_when_forum_is_not_valid()
        {
            var command = Fixture.Create<CreateTopic>();

            var dispatcher = new Mock<IDispatcher>();
            dispatcher.Setup(x => x.Get(new IsForumValid { SiteId = command.SiteId, Id = command.ForumId })).ReturnsAsync(false);

            var sut = new CreateTopicValidator(dispatcher.Object);

            sut.ShouldHaveValidationErrorFor(x => x.ForumId, command);
        }
    }
}
