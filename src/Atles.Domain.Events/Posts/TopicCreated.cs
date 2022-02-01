using Atles.Core.Events;
using Atles.Domain.Models;

namespace Atles.Domain.Events.Posts
{
    /// <summary>
    /// Event published when a topic is created.
    /// </summary>
    public class TopicCreated : EventBase
    {
        /// <summary>
        /// The unique identifier of the forum.
        /// </summary>
        public Guid ForumId { get; set; }

        /// <summary>
        /// The title of the topic.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The slug of the topic.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// The content of the topic.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The status of the topic.
        /// </summary>
        public PostStatusType Status { get; set; }
    }
}
