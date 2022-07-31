using Atles.Commands.Subscriptions;
using Atles.Core;
using Atles.Domain;
using Atles.Domain.Rules.Categories;
using Atles.Domain.Rules.Forums;
using Atles.Domain.Rules.Posts;
using Atles.Validators.Subscriptions;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Validators
{
    [Ignore("Refactoring needed")]
    [TestFixture]
    public class AddSubscriptionValidatorTests : TestFixtureBase
    {
        [Test]
        public async Task Should_have_validation_error_when_item_id_is_empty()
        {
            var command = Fixture.Build<AddSubscription>().With(x => x.ItemId, Guid.Empty).Create();

            var dispatcher = new Mock<IDispatcher>();

            var sut = new AddSubscriptionValidator(dispatcher.Object);

            var result = await sut.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.ItemId);
        }

        [Test]
        public async Task Should_have_validation_error_when_category_is_not_valid()
        {
            var command = Fixture.Build<AddSubscription>().With(x => x.Type, SubscriptionType.Category).Create();

            var dispatcher = new Mock<IDispatcher>();
            dispatcher
                .Setup(x => x.Get(new IsCategoryValid { SiteId = command.SiteId, Id = command.ItemId }))
                .ReturnsAsync(false);

            var sut = new AddSubscriptionValidator(dispatcher.Object);

            var result = await sut.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.ItemId);
        }

        [Test]
        public async Task Should_have_validation_error_when_forum_is_not_valid()
        {
            var command = Fixture.Build<AddSubscription>().With(x => x.Type, SubscriptionType.Forum).Create();

            var dispatcher = new Mock<IDispatcher>();
            dispatcher.Setup(x => x.Get(new IsForumValid { SiteId = command.SiteId, Id = command.ItemId })).ReturnsAsync(false);

            var sut = new AddSubscriptionValidator(dispatcher.Object);
            var result = await sut.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.ItemId);
        }

        [Test]
        public async Task Should_have_validation_error_when_topic_is_not_valid()
        {
            var command = Fixture.Build<AddSubscription>().With(x => x.Type, SubscriptionType.Topic).Create();

            var dispatcher = new Mock<IDispatcher>();
            dispatcher.Setup(x => x.Get(new IsTopicValid { SiteId = command.SiteId, ForumId = command.ForumId, Id = command.ItemId })).ReturnsAsync(false);

            var sut = new AddSubscriptionValidator(dispatcher.Object);

            var result = await sut.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.ItemId);
        }
    }
}
