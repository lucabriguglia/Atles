using Atles.Core.Commands;
using Atles.Domain.Models;
using Docs.Attributes;

namespace Atles.Domain.Commands;

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