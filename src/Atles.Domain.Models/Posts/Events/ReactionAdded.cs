using System;
using Atles.Domain.Models.PostReactions;
using Atles.Infrastructure.Events;

namespace Atles.Domain.Models.Posts.Events
{
    public class ReactionAdded : EventBase
    {
        public PostReactionType Type { get; set; }
    }
}
