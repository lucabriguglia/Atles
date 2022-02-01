using Atles.Core.Events;
using Atles.Domain.Models;

namespace Atles.Domain.Events.Posts
{
    public class PostReactionRemoved : EventBase
    {
        public PostReactionType Type { get; set; }
    }
}
