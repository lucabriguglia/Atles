using Atles.Core.Events;
using Atles.Domain.Models;

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
