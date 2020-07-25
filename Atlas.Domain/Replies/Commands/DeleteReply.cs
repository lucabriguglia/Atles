using System;

namespace Atlas.Domain.Replies.Commands
{
    public class DeleteReply : CommandBase
    {
        public Guid Id { get; set; }
        public Guid ForumId { get; set; }
        public Guid TopicId { get; set; }
    }
}
