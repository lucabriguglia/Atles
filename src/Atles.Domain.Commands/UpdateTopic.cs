using Atles.Core.Commands;
using Atles.Domain.Models;
using Docs.Attributes;

namespace Atles.Domain.Commands
{
    /// <summary>
    /// Request to update a topic.
    /// </summary>
    [DocRequest(typeof(Post))]
    public class UpdateTopic : CommandBase
    {
        public Guid TopicId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The unique identifier of the forum.
        /// </summary>
        public Guid ForumId { get; set; }

        /// <summary>
        /// The new title of the topic.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The new content of the topic.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The new status of the topic.
        /// </summary>
        public PostStatusType Status { get; set; }
    }
}
