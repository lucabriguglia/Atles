using Atles.Commands.Subscriptions;
using Atles.Core;
using Atles.Domain;
using Atles.Domain.Rules.Categories;
using Atles.Domain.Rules.Forums;
using Atles.Domain.Rules.Posts;
using FluentValidation;

namespace Atles.Validators.Subscriptions
{
    public class AddSubscriptionValidator : AbstractValidator<AddSubscription>
    {
        public AddSubscriptionValidator(IDispatcher dispatcher)
        {
            RuleFor(c => c.ItemId)
                .NotEmpty()
                .WithMessage("Item Id is required.");

            RuleFor(c => c.ItemId)
                .MustAsync(async (c, p, cancellation) =>
                {
                    // TODO: To be moved to a service
                    var result = await dispatcher.Get(new IsCategoryValid {SiteId = c.SiteId, Id = p});
                    return result.AsT0;
                })
                .When(c => c.Type == SubscriptionType.Category)
                .WithMessage(c => $"Category with id {c.ItemId} does not exist.");

            RuleFor(c => c.ItemId)
                .MustAsync(async (c, p, cancellation) =>
                {
                    // TODO: To be moved to a service
                    var result = await dispatcher.Get(new IsForumValid {SiteId = c.SiteId, Id = p});
                    return result.AsT0;
                })
                .When(c => c.Type == SubscriptionType.Forum)
                .WithMessage(c => $"Forum with id {c.ItemId} does not exist.");

            RuleFor(c => c.ItemId)
                .MustAsync(async (c, p, cancellation) =>
                {
                    // TODO: To be moved to a service
                    var result = await dispatcher.Get(new IsTopicValid {SiteId = c.SiteId, ForumId = c.ForumId, Id = p});
                    return result.AsT0;
                })
                .When(c => c.Type == SubscriptionType.Topic)
                .WithMessage(c => $"Topic with id {c.ItemId} does not exist.");
        }
    }
}
