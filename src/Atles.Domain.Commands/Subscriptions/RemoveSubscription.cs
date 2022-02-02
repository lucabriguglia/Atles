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
    public Guid RemoveSubscriptionUserId { get; set; }
    public Guid TargetId { get; set; }
}