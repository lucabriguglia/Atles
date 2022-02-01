using System;
using Atles.Infrastructure.Commands;
using Docs.Attributes;

namespace Atles.Domain.Models.Subscriptions.Commands;

/// <summary>
/// Request to remove a subscription.
/// </summary>
[DocRequest(typeof(Subscription))]
public class RemoveSubscription : CommandBase
{
    /// <summary>
    /// TargetId
    /// </summary>
    public Guid TargetId { get; set; }
}