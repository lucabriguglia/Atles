using Atles.Core.Events;

namespace Atles.Domain.Events.Posts
{
    public class PostReactionRemoved : EventBase
    {
        public PostReactionType Type { get; set; }
    }
}
