using Atles.Core.Commands;
using Atles.Domain.Models;
using Docs.Attributes;

namespace Atles.Domain.Commands.Subscriptions;

/// <summary>
/// Request to remove a subscription.
/// </summary>
[DocRequest(typeof(Subscription))]
public class RemoveSubscription : CommandBase
{
    /// <summary>
    /// The id of the item (category, forum or topic).
    /// </summary>
    public Guid ItemId { get; set; }
}