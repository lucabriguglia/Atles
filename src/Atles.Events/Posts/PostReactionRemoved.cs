using Atles.Core.Events;
using Atles.Domain;

namespace Atles.Events.Posts
{
    public class PostReactionRemoved : EventBase
    {
        public PostReactionType Type { get; set; }
    }
}
