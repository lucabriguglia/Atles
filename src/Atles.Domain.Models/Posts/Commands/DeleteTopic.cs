using Atles.Infrastructure.Commands;
using System;
using Docs.Attributes;

namespace Atles.Domain.Posts.Commands
{
    /// <summary>
    /// Request to delete a topic.
    /// </summary>
    [DocRequest(typeof(Post))]
    public class DeleteTopic : CommandBase
    {
        /// <summary>
        /// The unique identifier of the forum.
        /// </summary>
        public Guid ForumId { get; set; }
    }
}
