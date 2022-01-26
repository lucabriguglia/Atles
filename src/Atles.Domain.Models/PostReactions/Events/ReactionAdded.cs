using System;
using Atles.Infrastructure.Events;

namespace Atles.Domain.Models.PostReactions.Events
{
    public class ReactionAdded : EventBase
    {
        public Guid PostId { get; set; }
        public PostReactionType Type { get; set; }
    }
}
