using System;
using Atles.Domain.Models.Users;
using Docs.Attributes;

namespace Atles.Domain.Models.Subscriptions
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
        /// TargetId
        /// </summary>
        public Guid TargetId { get; private set; }

        /// <summary>
        /// Type of subscription
        /// </summary>
        public SubscriptionType Type { get; private set; }

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
        /// <param name="targetId"></param>
        /// <param name="type"></param>
        public Subscription(Guid userId, Guid targetId, SubscriptionType type)
        {
            UserId = userId;
            TargetId = targetId;
            Type = type;
        }
    }
}
