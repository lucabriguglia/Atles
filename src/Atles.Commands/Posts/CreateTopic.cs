using Atles.Core.Commands;
using Atles.Domain;

namespace Atles.Commands.Posts
{
    /// <summary>
    /// Request to create a new topic.
    /// </summary>
    public class CreateTopic : CommandBase
    {
        public Guid TopicId { get; set; } = Guid.NewGuid();

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

        /// <summary>
        /// Receive notifications for replies.
        /// </summary>
        public bool Subscribe { get; set; }
    }
}
