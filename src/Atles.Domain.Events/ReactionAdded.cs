using Atles.Core.Events;
using Atles.Domain.Models;

namespace Atles.Domain.Events
{
    public class ReactionAdded : EventBase
    {
        public PostReactionType Type { get; set; }
    }
}
