using Atles.Commands.Subscriptions;
using Atles.Domain;
using Atles.Validators.ValidationRules;
using FluentValidation;

namespace Atles.Validators.Subscriptions;

public class AddSubscriptionValidator : AbstractValidator<AddSubscription>
{
    public AddSubscriptionValidator(ICategoryValidationRules categoryValidationRules, IForumValidationRules forumValidationRules, ITopicValidationRules topicValidationRules)
    {
        RuleFor(c => c.ItemId)
            .NotEmpty()
            .WithMessage("Item Id is required.");

        RuleFor(c => c.ItemId)
            .MustAsync(async (model, categoryId, cancellation) => await categoryValidationRules.IsCategoryValid(model.SiteId, categoryId))
            .When(c => c.Type == SubscriptionType.Category)
            .WithMessage(c => $"Category with id {c.ItemId} does not exist.");

        RuleFor(c => c.ItemId)
            .MustAsync(async (model, itemId, cancellation) => await forumValidationRules.IsForumValid(model.SiteId, itemId))
            .When(c => c.Type == SubscriptionType.Forum)
            .WithMessage(c => $"Forum with id {c.ItemId} does not exist.");

        RuleFor(c => c.ItemId)
            .MustAsync(async (c, p, cancellation) => await topicValidationRules.IsTopicValid(c.SiteId, c.ForumId, c.ItemId))
            .When(c => c.Type == SubscriptionType.Topic)
            .WithMessage(c => $"Topic with id {c.ItemId} does not exist.");
    }
}
