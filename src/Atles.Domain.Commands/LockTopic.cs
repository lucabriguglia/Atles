using System;
using Atles.Domain.Models.Posts;
using Atles.Infrastructure.Commands;
using Docs.Attributes;

namespace Atles.Domain.Commands
{
    /// <summary>
    /// Request to lock/unlock a topic.
    /// </summary>
    [DocRequest(typeof(Post))]
    public class LockTopic : CommandBase
    {
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