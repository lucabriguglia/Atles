using Atles.Core.Events;
using Atles.Domain;

namespace Atles.Events.Posts
{
    /// <summary>
    /// Event published when a reply is created.
    /// </summary>
    public class ReplyCreated : EventBase
    {
        /// <summary>
        /// The unique identifier of the forum.
        /// </summary>
        public Guid ForumId { get; set; }

        /// <summary>
        /// The unique identifier of the topic.
        /// </summary>
        public Guid TopicId { get; set; }

        /// <summary>
        /// The content of the reply.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The status of the reply.
        /// </summary>
        public PostStatusType Status { get; set; }
    }
}
