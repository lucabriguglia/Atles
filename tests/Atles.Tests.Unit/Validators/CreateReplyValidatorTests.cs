using Atles.Commands.Posts;
using Atles.Core;
using Atles.Domain.Rules.Posts;
using Atles.Validators.Forums;
using Atles.Validators.Posts;
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

        var dispatcher = new Mock<IDispatcher>();
        dispatcher.Setup(x => x.Get(new IsTopicValid { SiteId = model.SiteId, ForumId = model.ForumId, Id = model.TopicId })).ReturnsAsync(true);

        var forumValidationRules = new Mock<IForumValidationRules>();

        var sut = new CreateReplyValidator(dispatcher.Object, forumValidationRules.Object);

        var result = await sut.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    [Test]
    public async Task Should_have_validation_error_when_forum_is_not_valid()
    {
        var model = Fixture.Create<CreateReply>();

        var dispatcher = new Mock<IDispatcher>();
        dispatcher.Setup(x => x.Get(new IsTopicValid { SiteId = model.SiteId, ForumId = model.ForumId, Id = model.TopicId })).ReturnsAsync(true);

        var forumValidationRules = new Mock<IForumValidationRules>();
        forumValidationRules
            .Setup(rules => rules.IsForumValid(model.SiteId, model.ForumId))
            .ReturnsAsync(false);

        var sut = new CreateReplyValidator(dispatcher.Object, forumValidationRules.Object);

        var result = await sut.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.ForumId);
    }

    [Test]
    public async Task Should_have_validation_error_when_topic_is_not_valid()
    {
        var model = Fixture.Create<CreateReply>();

        var dispatcher = new Mock<IDispatcher>();
        dispatcher.Setup(x => x.Get(new IsTopicValid { SiteId = model.SiteId, ForumId = model.ForumId, Id = model.TopicId })).ReturnsAsync(false);
        
        var forumValidationRules = new Mock<IForumValidationRules>();

        var sut = new CreateReplyValidator(dispatcher.Object, forumValidationRules.Object);

        var result = await sut.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.TopicId);
    }
}
