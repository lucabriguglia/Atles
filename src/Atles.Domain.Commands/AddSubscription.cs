using Atles.Core.Commands;
using Atles.Domain.Models;
using Docs.Attributes;

namespace Atles.Domain.Commands
{
    /// <summary>
    /// Request to add a subscription.
    /// </summary>
    [DocRequest(typeof(Subscription))]
    public class AddSubscription : CommandBase
    {
        /// <summary>
        /// Type of subscription
        /// </summary>
        public SubscriptionType Type { get; set; }

        /// <summary>
        /// SubscriptionUserId
        /// </summary>
        public Guid AddSubscriptionUserId { get; set; }

        /// <summary>
        /// TargetId
        /// </summary>
        public Guid TargetId { get; set; }
    }
}
