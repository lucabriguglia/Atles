using Atles.Domain.Models.PostReactions;
using Atles.Infrastructure.Events;
using Docs.Attributes;

namespace Atles.Domain.Models.Posts.Events
{
    [DocRequest(typeof(PostReactionSummary))]
    public class ReactionRemoved : EventBase
    {
        public PostReactionType Type { get; set; }
    }
}
