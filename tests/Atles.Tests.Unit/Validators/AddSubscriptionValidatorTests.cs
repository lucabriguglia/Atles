using Atles.Commands.Subscriptions;
using Atles.Domain;
using Atles.Validators.Subscriptions;
using Atles.Validators.ValidationRules;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Validators
{
    [TestFixture]
    public class AddSubscriptionValidatorTests : TestFixtureBase
    {
        [Test]
        public async Task Should_have_validation_error_when_item_id_is_empty()
        {
            var command = Fixture.Build<AddSubscription>().With(x => x.ItemId, Guid.Empty).Create();

            var categoryValidationRules = new Mock<ICategoryValidationRules>();
            var forumValidationRules = new Mock<IForumValidationRules>();
            var topicValidationRules = new Mock<ITopicValidationRules>();

            var sut = new AddSubscriptionValidator(categoryValidationRules.Object, forumValidationRules.Object, topicValidationRules.Object);

            var result = await sut.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.ItemId);
        }

        [Test]
        public async Task Should_have_validation_error_when_category_is_not_valid()
        {
            var command = Fixture.Build<AddSubscription>().With(x => x.Type, SubscriptionType.Category).Create();

            var categoryValidationRules = new Mock<ICategoryValidationRules>();
            categoryValidationRules
                .Setup(x => x.IsCategoryValid(command.SiteId, command.ItemId))
                .ReturnsAsync(false);
            var forumValidationRules = new Mock<IForumValidationRules>();
            var topicValidationRules = new Mock<ITopicValidationRules>();

            var sut = new AddSubscriptionValidator(categoryValidationRules.Object, forumValidationRules.Object, topicValidationRules.Object);

            var result = await sut.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.ItemId);
        }

        [Test]
        public async Task Should_have_validation_error_when_forum_is_not_valid()
        {
            var command = Fixture.Build<AddSubscription>().With(x => x.Type, SubscriptionType.Forum).Create();

            var categoryValidationRules = new Mock<ICategoryValidationRules>();
            var forumValidationRules = new Mock<IForumValidationRules>();
            forumValidationRules
                .Setup(x => x.IsForumValid(command.SiteId, command.ItemId))
                .ReturnsAsync(false);
            var topicValidationRules = new Mock<ITopicValidationRules>();

            var sut = new AddSubscriptionValidator(categoryValidationRules.Object, forumValidationRules.Object, topicValidationRules.Object);

            var result = await sut.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.ItemId);
        }

        [Test]
        public async Task Should_have_validation_error_when_topic_is_not_valid()
        {
            var model = Fixture.Build<AddSubscription>().With(x => x.Type, SubscriptionType.Topic).Create();

            var categoryValidationRules = new Mock<ICategoryValidationRules>();
            var forumValidationRules = new Mock<IForumValidationRules>();
            var topicValidationRules = new Mock<ITopicValidationRules>();
            topicValidationRules
                .Setup(rules => rules.IsTopicValid(model.SiteId, model.ForumId, model.ItemId))
                .ReturnsAsync(false);

            var sut = new AddSubscriptionValidator(categoryValidationRules.Object, forumValidationRules.Object, topicValidationRules.Object);

            var result = await sut.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.ItemId);
        }
    }
}
