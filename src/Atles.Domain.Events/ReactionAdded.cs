using Atles.Domain.Models;
using Atles.Infrastructure.Events;

namespace Atles.Domain.Events
{
    public class ReactionAdded : EventBase
    {
        public PostReactionType Type { get; set; }
    }
}
