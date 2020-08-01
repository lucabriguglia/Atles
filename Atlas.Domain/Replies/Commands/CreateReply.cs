using System;

namespace Atlas.Domain.Replies.Commands
{
    public class CreateReply : CommandBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ForumId { get; set; }
        public Guid TopicId { get; set; }
        public string Content { get; set; }
        public StatusType Status { get; set; }
    }
}
