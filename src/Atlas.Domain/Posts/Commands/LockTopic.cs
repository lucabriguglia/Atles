using System;
using Docs.Attributes;

namespace Atlas.Domain.Posts.Commands
{
    /// <summary>
    /// Request to lock/unlock a topic.
    /// </summary>
    [DocRequest(typeof(Post))]
    public class LockTopic : CommandBase
    {
        /// <summary>
        /// The unique identifier of the topic to lock/unlock.
        /// </summary>
        public Guid Id { get; set; }

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