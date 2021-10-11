using Atles.Domain.Forums.Rules;
using Atles.Domain.Posts.Commands;
using Atles.Domain.Posts.Rules;
using Atles.Domain.Posts.Validators;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using OpenCqrs;

namespace Atles.Domain.Tests.Replies.Validators
{
    [TestFixture]
    public class CreateReplyValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_content_is_empty()
        {
            var command = Fixture.Build<CreateReply>().With(x => x.Content, string.Empty).Create();

            var sender = new Mock<ISender>();

            var sut = new CreateReplyValidator(sender.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Content, command);
        }

        [Test]
        public void Should_have_validation_error_when_forum_is_not_valid()
        {
            var command = Fixture.Create<CreateReply>();

            var sender = new Mock<ISender>();
            sender.Setup(x => x.Send(new IsForumValid { SiteId = command.SiteId, Id = command.ForumId })).ReturnsAsync(false);

            var sut = new CreateReplyValidator(sender.Object);

            sut.ShouldHaveValidationErrorFor(x => x.ForumId, command);
        }

        [Test]
        public void Should_have_validation_error_when_topic_is_not_valid()
        {
            var command = Fixture.Create<CreateReply>();

            var sender = new Mock<ISender>();
            sender.Setup(x => x.Send(new IsTopicValid { SiteId = command.SiteId, ForumId = command.ForumId, Id = command.TopicId })).ReturnsAsync(false);

            var sut = new CreateReplyValidator(sender.Object);

            sut.ShouldHaveValidationErrorFor(x => x.TopicId, command);
        }
    }
}
