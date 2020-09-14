using System;
using Docs.Attributes;

namespace Atlas.Domain.Posts.Commands
{
    /// <summary>
    /// Request to delete a topic.
    /// </summary>
    [DocRequest(typeof(Post))]
    public class DeleteTopic : CommandBase
    {
        /// <summary>
        /// The unique identifier of the topic to delete.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The unique identifier of the forum.
        /// </summary>
        public Guid ForumId { get; set; }
    }
}
