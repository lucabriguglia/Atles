using Atles.Infrastructure.Events;

namespace Atles.Domain.Models.Subscriptions.Events
{
    /// <summary>
    /// Event published when a new subscription is added.
    /// </summary>
    public class SubscriptionAdded : EventBase
    {
        /// <summary>
        /// Type of subscription
        /// </summary>
        public SubscriptionType Type { get; set; }
    }
}
