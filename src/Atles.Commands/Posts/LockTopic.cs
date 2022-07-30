using Atles.Core.Commands;

namespace Atles.Commands.Posts
{
    /// <summary>
    /// Request to lock/unlock a topic.
    /// </summary>
    public class LockTopic : CommandBase
    {
        public Guid TopicId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The unique identifier of the forum.
        /// </summary>
        public Guid ForumId { get; set; }

        /// <summary>
        /// Value indicating whether the topic needs to be locked (true) or unlocked (false).
        /// </summary>
        public bool Locked { get; set; }
    }
}