using Atles.Commands.Posts;
using Atles.Core;
using Atles.Validators.Posts;
using Atles.Validators.ValidationRules;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Validators;

[TestFixture]
public class CreateTopicValidatorTests : TestFixtureBase
{
    [Test]
    public async Task Should_have_validation_error_when_title_is_empty()
    {
        var model = Fixture.Build<CreateTopic>().With(x => x.Title, string.Empty).Create();

        var dispatcher = new Mock<IDispatcher>();
        var forumValidationRules = new Mock<IForumValidationRules>();

        var sut = new CreateTopicValidator(dispatcher.Object, forumValidationRules.Object);

        var result = await sut.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Test]
    public async Task Should_have_validation_error_when_title_is_too_long()
    {
        var model = Fixture.Build<CreateTopic>().With(x => x.Title, new string('*', 101)).Create();

        var dispatcher = new Mock<IDispatcher>();
        var forumValidationRules = new Mock<IForumValidationRules>();

        var sut = new CreateTopicValidator(dispatcher.Object, forumValidationRules.Object);

        var result = await sut.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Test]
    public async Task Should_have_validation_error_when_content_is_empty()
    {
        var model = Fixture.Build<CreateTopic>().With(x => x.Content, string.Empty).Create();

        var dispatcher = new Mock<IDispatcher>();
        var forumValidationRules = new Mock<IForumValidationRules>();

        var sut = new CreateTopicValidator(dispatcher.Object, forumValidationRules.Object);

        var result = await sut.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    [Test]
    public async Task Should_have_validation_error_when_forum_is_not_valid()
    {
        var model = Fixture.Create<CreateTopic>();

        var dispatcher = new Mock<IDispatcher>();
        var forumValidationRules = new Mock<IForumValidationRules>();
        forumValidationRules
            .Setup(rules => rules.IsForumValid(model.SiteId, model.ForumId))
            .ReturnsAsync(false);

        var sut = new CreateTopicValidator(dispatcher.Object, forumValidationRules.Object);

        var result = await sut.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.ForumId);
    }
}
