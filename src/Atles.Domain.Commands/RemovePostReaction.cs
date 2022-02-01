using Atles.Core.Commands;
using Atles.Domain.Models;
using Docs.Attributes;

namespace Atles.Domain.Commands
{
    [DocRequest(typeof(Post))]
    public class RemovePostReaction : CommandBase
    {
        public Guid ForumId { get; set; }
    }
}
