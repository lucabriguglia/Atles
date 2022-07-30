using Atles.Core.Commands;

namespace Atles.Commands.Posts
{
    public class RemovePostReaction : CommandBase
    {
        public Guid PostId { get; set; } = Guid.NewGuid();

        public Guid ForumId { get; set; }
    }
}
