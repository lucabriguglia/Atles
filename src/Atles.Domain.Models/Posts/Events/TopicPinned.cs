using Atles.Infrastructure.Events;

namespace Atles.Domain.Models.Posts.Events
{
    /// <summary>
    /// Event published when a topic is pinned.
    /// </summary>
    public class TopicPinned : EventBase
    {
        /// <summary>
        /// Value indicating whether the topic is pinned (true) or unpinned (false).
        /// </summary>
        public bool Pinned { get; set; }
    }
}