using Atles.Core.Events;
using Atles.Domain;

namespace Atles.Events
{
    /// <summary>
    /// Event published when a new subscription is removed.
    /// </summary>
    public class SubscriptionRemoved : EventBase
    {
        /// <summary>
        /// The id of the item (category, forum or topic).
        /// </summary>
        public Guid ItemId { get; set; }

        /// <summary>
        /// The type of subscription  (category, forum or topic).
        /// </summary>
        public SubscriptionType Type { get; set; }
    }
}