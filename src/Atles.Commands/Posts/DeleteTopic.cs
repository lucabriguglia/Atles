using Atles.Core.Commands;

namespace Atles.Commands.Posts
{
    /// <summary>
    /// Request to delete a topic.
    /// </summary>
    public class DeleteTopic : CommandBase
    {
        public Guid TopicId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The unique identifier of the forum.
        /// </summary>
        public Guid ForumId { get; set; }
    }
}
