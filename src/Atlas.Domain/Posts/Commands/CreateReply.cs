using System;
using Docs.Attributes;

namespace Atlas.Domain.Posts.Commands
{
    /// <summary>
    /// Request to create a new reply.
    /// </summary>
    [DocRequest(typeof(Post))]
    public class CreateReply : CommandBase
    {
        /// <summary>
        /// The unique identifier of the reply.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

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
        public StatusType Status { get; set; }
    }
}
