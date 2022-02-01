using Atles.Core.Events;
using Atles.Domain.Models;

namespace Atles.Domain.Events.Posts
{
    public class PostReactionAdded : EventBase
    {
        public PostReactionType Type { get; set; }
    }
}
