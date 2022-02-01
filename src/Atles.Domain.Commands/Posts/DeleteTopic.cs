using Atles.Core.Commands;
using Atles.Domain.Models;
using Docs.Attributes;

namespace Atles.Domain.Commands.Posts
{
    /// <summary>
    /// Request to delete a topic.
    /// </summary>
    [DocRequest(typeof(Post))]
    public class DeleteTopic : CommandBase
    {
        public Guid TopicId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The unique identifier of the forum.
        /// </summary>
        public Guid ForumId { get; set; }
    }
}
