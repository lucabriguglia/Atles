using Atles.Core.Commands;
using Atles.Domain.Models;

namespace Atles.Commands.Posts
{
    public class AddPostReaction : CommandBase
    {
        public Guid PostId { get; set; } = Guid.NewGuid();
        public Guid ForumId { get; set; }
        public PostReactionType Type { get; set; }
    }
}
