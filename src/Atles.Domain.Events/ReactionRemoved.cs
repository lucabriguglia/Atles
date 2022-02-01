using Atles.Domain.Models;
using Atles.Infrastructure.Events;
using Docs.Attributes;

namespace Atles.Domain.Events
{
    [DocRequest(typeof(PostReactionSummary))]
    public class ReactionRemoved : EventBase
    {
        public PostReactionType Type { get; set; }
    }
}
