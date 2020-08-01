using System;

namespace Atlas.Domain.Replies.Commands
{
    public class UpdateReply : CommandBase
    {
        public Guid Id { get; set; }
        public Guid ForumId { get; set; }
        public Guid TopicId { get; set; }
        public string Content { get; set; }
        public StatusType Status { get; set; }
    }
}
