using Atles.Domain.Models;
using Atles.Infrastructure.Commands;
using Docs.Attributes;

namespace Atles.Domain.Commands
{
    [DocRequest(typeof(PostReactionSummary))]
    public class RemoveReaction : CommandBase
    {
        public Guid ForumId { get; set; }
    }
}
