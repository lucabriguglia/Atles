using Atles.Core.Commands;
using Atles.Domain;

namespace Atles.Commands.Subscriptions
{
    /// <summary>
    /// Request to add a subscription.
    /// </summary>
    public class AddSubscription : CommandBase
    {
        /// <summary>
        /// The type of subscription  (category, forum or topic).
        /// </summary>
        public SubscriptionType Type { get; set; }

        /// <summary>
        /// The id of the item (category, forum or topic).
        /// </summary>
        public Guid ItemId { get; set; }

        /// <summary>
        /// The id of the forum.
        /// </summary>
        public Guid ForumId { get; set; }
    }
}
