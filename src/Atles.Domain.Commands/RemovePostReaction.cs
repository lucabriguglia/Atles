using Atles.Core.Commands;
using Atles.Domain.Models;
using Docs.Attributes;

namespace Atles.Domain.Commands
{
    [DocRequest(typeof(Post))]
    public class RemovePostReaction : CommandBase
    {
        public Guid PostId { get; set; } = Guid.NewGuid();

        public Guid ForumId { get; set; }
    }
}
