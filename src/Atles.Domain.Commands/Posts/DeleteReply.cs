using Atles.Core.Commands;
using Atles.Domain.Models;
using Docs.Attributes;

namespace Atles.Domain.Commands.Posts
{
    /// <summary>
    /// Request to delete a reply.
    /// </summary>
    [DocRequest(typeof(Post))]
    public class DeleteReply : CommandBase
    {
        public Guid ReplyId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The unique identifier of the forum.
        /// </summary>
        public Guid ForumId { get; set; }

        /// <summary>
        /// The unique identifier of the topic.
        /// </summary>
        public Guid TopicId { get; set; }
    }
}
