using Atles.Core.Commands;
using Atles.Domain.Models;
using Docs.Attributes;

namespace Atles.Domain.Commands
{
    /// <summary>
    /// Request to lock/unlock a topic.
    /// </summary>
    [DocRequest(typeof(Post))]
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