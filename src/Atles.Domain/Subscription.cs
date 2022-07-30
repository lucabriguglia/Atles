using System;
using Docs.Attributes;

namespace Atles.Domain
{
    /// <summary>
    /// Subscription
    /// </summary>
    [DocTarget(nameof(Subscription))]
    public class Subscription
    {
        /// <summary>
        /// UserId
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Type of subscription
        /// </summary>
        public SubscriptionType Type { get; private set; }

        /// <summary>
        /// TargetId
        /// </summary>
        public Guid ItemId { get; private set; }

        /// <summary>
        /// Reference to the user who added the subscription.
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Subscription
        /// </summary>
        public Subscription()
        {
        }

        /// <summary>
        /// Subscription
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="type"></param>
        /// <param name="itemId"></param>
        public Subscription(Guid userId, SubscriptionType type, Guid itemId)
        {
            UserId = userId;
            Type = type;
            ItemId = itemId;
        }
    }
}
