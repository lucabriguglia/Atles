using Atles.Domain.Forums;
using Atles.Domain.Posts;
using Atles.Domain.Posts.Commands;
using Atles.Domain.Posts.Validators;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Atlas.Domain.Tests.Replies.Validators
{
    [TestFixture]
    public class CreateReplyValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_content_is_empty()
        {
            var command = Fixture.Build<CreateReply>().With(x => x.Content, string.Empty).Create();

            var forumRules = new Mock<IForumRules>();
            var topicRules = new Mock<ITopicRules>();

            var sut = new CreateReplyValidator(forumRules.Object, topicRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Content, command);
        }

        [Test]
        public void Should_have_validation_error_when_forum_is_not_valid()
        {
            var command = Fixture.Create<CreateReply>();

            var forumRules = new Mock<IForumRules>();
            forumRules.Setup(x => x.IsValidAsync(command.SiteId, command.ForumId)).ReturnsAsync(false);

            var topicRules = new Mock<ITopicRules>();

            var sut = new CreateReplyValidator(forumRules.Object, topicRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.ForumId, command);
        }

        [Test]
        public void Should_have_validation_error_when_topic_is_not_valid()
        {
            var command = Fixture.Create<CreateReply>();

            var forumRules = new Mock<IForumRules>();

            var topicRules = new Mock<ITopicRules>();
            topicRules.Setup(x => x.IsValidAsync(command.SiteId, command.ForumId, command.TopicId)).ReturnsAsync(false);

            var sut = new CreateReplyValidator(forumRules.Object, topicRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.TopicId, command);
        }
    }
}
