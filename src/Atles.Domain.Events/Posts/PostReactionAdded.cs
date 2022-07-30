using Atles.Core.Events;

namespace Atles.Domain.Events.Posts
{
    public class PostReactionAdded : EventBase
    {
        public PostReactionType Type { get; set; }
    }
}
