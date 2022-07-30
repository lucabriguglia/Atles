using Atles.Core.Commands;

namespace Atles.Commands.Subscriptions;

/// <summary>
/// Request to remove a subscription.
/// </summary>
public class RemoveSubscription : CommandBase
{
    /// <summary>
    /// The id of the item (category, forum or topic).
    /// </summary>
    public Guid ItemId { get; set; }
}