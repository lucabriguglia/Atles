using Atles.Domain.Models;
using Atles.Infrastructure.Commands;
using Docs.Attributes;

namespace Atles.Domain.Commands
{
    /// <summary>
    /// Request to create a new topic.
    /// </summary>
    [DocRequest(typeof(Post))]
    public class CreateTopic : CommandBase
    {
        /// <summary>
        /// The unique identifier of the forum.
        /// </summary>
        public Guid ForumId { get; set; }

        /// <summary>
        /// The title of the topic.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The content of the topic.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The status of the topic.
        /// </summary>
        public PostStatusType Status { get; set; }
    }
}
