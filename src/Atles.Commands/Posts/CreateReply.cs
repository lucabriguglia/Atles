using Atles.Core.Commands;
using Atles.Domain;

namespace Atles.Commands.Posts
{
    /// <summary>
    /// Request to create a new reply.
    /// </summary>
    public class CreateReply : CommandBase
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

        /// <summary>
        /// The content of the reply.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The status of the reply.
        /// </summary>
        public PostStatusType Status { get; set; }
    }
}
