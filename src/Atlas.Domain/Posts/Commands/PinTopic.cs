using System;

namespace Atlas.Domain.Posts.Commands
{
    public class PinTopic : CommandBase
    {
        public Guid Id { get; set; }
        public Guid ForumId { get; set; }
        public bool Pinned { get; set; }
    }
}