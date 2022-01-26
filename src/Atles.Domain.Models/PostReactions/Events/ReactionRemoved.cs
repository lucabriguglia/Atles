using System;
using Atles.Infrastructure.Events;
using Docs.Attributes;

namespace Atles.Domain.Models.PostReactions.Events
{
    [DocRequest(typeof(PostReaction))]
    public class ReactionRemoved : EventBase
    {
        public Guid PostId { get; set; }
    }
}
