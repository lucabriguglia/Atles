using Atles.Core.Events;
using Atles.Domain;

namespace Atles.Events.Posts
{
    public class PostReactionAdded : EventBase
    {
        public PostReactionType Type { get; set; }
    }
}
