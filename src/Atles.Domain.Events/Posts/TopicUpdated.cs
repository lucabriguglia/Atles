using Atles.Core.Events;
using Atles.Domain;

namespace Atles.Events.Posts
{
    /// <summary>
    /// Event published when a topic is updated.
    /// </summary>
    public class TopicUpdated : EventBase
    {
        /// <summary>
        /// The new title of the topic.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The new slug of the topic.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// The new content of the topic.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The new status of the topic.
        /// </summary>
        public PostStatusType Status { get; set; }
    }
}
