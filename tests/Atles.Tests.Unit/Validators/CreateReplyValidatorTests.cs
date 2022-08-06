using Atles.Commands.Posts;
using Atles.Validators.Posts;
using Atles.Validators.ValidationRules;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Validators;

[TestFixture]
public class CreateReplyValidatorTests : TestFixtureBase
{
    [Test]
    public async Task Should_have_validation_error_when_content_is_empty()
    {
        var model = Fixture.Build<CreateReply>().With(x => x.Content, string.Empty).Create();

        var forumValidationRules = new Mock<IForumValidationRules>();
        var topicValidationRules = new Mock<ITopicValidationRules>();

        var sut = new CreateReplyValidator(forumValidationRules.Object, topicValidationRules.Object);

        var result = await sut.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    [Test]
    public async Task Should_have_validation_error_when_forum_is_not_valid()
    {
        var model = Fixture.Create<CreateReply>();

        var forumValidationRules = new Mock<IForumValidationRules>();
        forumValidationRules
            .Setup(rules => rules.IsForumValid(model.SiteId, model.ForumId))
            .ReturnsAsync(false);
        var topicValidationRules = new Mock<ITopicValidationRules>();

        var sut = new CreateReplyValidator(forumValidationRules.Object, topicValidationRules.Object);

        var result = await sut.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.ForumId);
    }

    [Test]
    public async Task Should_have_validation_error_when_topic_is_not_valid()
    {
        var model = Fixture.Create<CreateReply>();

        var forumValidationRules = new Mock<IForumValidationRules>();
        var topicValidationRules = new Mock<ITopicValidationRules>();
        topicValidationRules
            .Setup(rules => rules.IsTopicValid(model.SiteId, model.ForumId, model.TopicId))
            .ReturnsAsync(false);

        var sut = new CreateReplyValidator(forumValidationRules.Object, topicValidationRules.Object);

        var result = await sut.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.TopicId);
    }
}
