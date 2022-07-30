using Atles.Core.Events;

namespace Atles.Domain.Events.Posts
{
    /// <summary>
    /// Event published when a reply is updated.
    /// </summary>
    public class ReplyUpdated : EventBase
    {
        /// <summary>
        /// The new content of the reply.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The new status of the reply.
        /// </summary>
        public PostStatusType Status { get; set; }
    }
}
