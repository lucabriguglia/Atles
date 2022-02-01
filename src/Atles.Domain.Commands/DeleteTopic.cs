using Atles.Core.Commands;
using Atles.Domain.Models;
using Docs.Attributes;

namespace Atles.Domain.Commands
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
