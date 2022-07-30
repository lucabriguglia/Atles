using Atles.Core.Commands;

namespace Atles.Commands.Forums
{
    /// <summary>
    /// Request that deletes a forum.
    /// </summary>
    public class DeleteForum : CommandBase
    {
        public Guid ForumId { get; set; } = Guid.NewGuid();
    }
}
