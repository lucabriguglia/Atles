using Atles.Commands.Subscriptions;
using Atles.Core;
using Atles.Domain.Models;
using Atles.Domain.Rules.Categories;
using Atles.Domain.Rules.Forums;
using Atles.Domain.Rules.Posts;
using FluentValidation;

namespace Atles.Domain.Commands.Handlers.Subscriptions.Validators
{
    public class AddSubscriptionValidator : AbstractValidator<AddSubscription>
    {
        public AddSubscriptionValidator(IDispatcher dispatcher)
        {
            RuleFor(c => c.ItemId)
                .NotEmpty()
                .WithMessage("Item Id is required.");

            RuleFor(c => c.ItemId)
                .MustAsync((c, p, cancellation) => dispatcher.Get(new IsCategoryValid { SiteId = c.SiteId, Id = p }))
                .When(c => c.Type == SubscriptionType.Category)
                .WithMessage(c => $"Category with id {c.ItemId} does not exist.");

            RuleFor(c => c.ItemId)
                .MustAsync((c, p, cancellation) => dispatcher.Get(new IsForumValid { SiteId = c.SiteId, Id = p }))
                .When(c => c.Type == SubscriptionType.Forum)
                .WithMessage(c => $"Forum with id {c.ItemId} does not exist.");

            RuleFor(c => c.ItemId)
                .MustAsync((c, p, cancellation) => dispatcher.Get(new IsTopicValid { SiteId = c.SiteId, ForumId = c.ForumId, Id = p }))
                .When(c => c.Type == SubscriptionType.Topic)
                .WithMessage(c => $"Topic with id {c.ItemId} does not exist.");
        }
    }
}
