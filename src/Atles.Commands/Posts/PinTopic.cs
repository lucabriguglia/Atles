using Atles.Core.Commands;

namespace Atles.Commands.Posts
{
    /// <summary>
    /// Request to pin/unpin a topic.
    /// </summary>
    public class PinTopic : CommandBase
    {
        public Guid TopicId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The unique identifier of the forum.
        /// </summary>
        public Guid ForumId { get; set; }

        /// <summary>
        /// Value indicating whether the topic needs to be pinned (true) or unpinned (false).
        /// </summary>
        public bool Pinned { get; set; }
    }
}