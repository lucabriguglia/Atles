using Atles.Core.Commands;

namespace Atles.Commands.Posts
{
    /// <summary>
    /// Request to delete a reply.
    /// </summary>
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
