using System;
using Docs.Attributes;

namespace Atlas.Domain.Posts.Commands
{
    /// <summary>
    /// Request to update a reply.
    /// </summary>
    [DocRequest(typeof(Post))]
    public class UpdateReply : CommandBase
    {
        /// <summary>
        /// The unique identifier of the reply to update.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The unique identifier of the forum.
        /// </summary>
        public Guid ForumId { get; set; }

        /// <summary>
        /// The unique identifier of the topic.
        /// </summary>
        public Guid TopicId { get; set; }

        /// <summary>
        /// The new content of the reply.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The new status of the reply.
        /// </summary>
        public PostStatusType Status { get; set; }
    }
}
