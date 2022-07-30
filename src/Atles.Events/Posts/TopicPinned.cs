using Atles.Core.Events;

namespace Atles.Events.Posts
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