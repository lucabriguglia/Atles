using Atles.Core.Commands;
using Atles.Domain.Models;
using Docs.Attributes;

namespace Atles.Domain.Commands
{
    /// <summary>
    /// Request to pin/unpin a topic.
    /// </summary>
    [DocRequest(typeof(Post))]
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