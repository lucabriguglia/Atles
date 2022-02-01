using Atles.Core.Commands;
using Atles.Domain.Models;
using Docs.Attributes;

namespace Atles.Domain.Commands.Posts
{
    [DocRequest(typeof(Post))]
    public class AddPostReaction : CommandBase
    {
        public Guid PostId { get; set; } = Guid.NewGuid();
        public Guid ForumId { get; set; }
        public PostReactionType Type { get; set; }
    }
}
