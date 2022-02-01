using Atles.Core.Events;
using Atles.Domain.Models;
using Docs.Attributes;

namespace Atles.Domain.Events
{
    [DocRequest(typeof(PostReactionSummary))]
    public class ReactionRemoved : EventBase
    {
        public PostReactionType Type { get; set; }
    }
}
