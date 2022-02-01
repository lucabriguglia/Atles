using Atles.Domain.Models;
using Atles.Infrastructure.Events;

namespace Atles.Domain.Events
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
