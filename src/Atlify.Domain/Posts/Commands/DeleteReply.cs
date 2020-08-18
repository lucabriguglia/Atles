using System;

namespace Atlify.Domain.Posts.Commands
{
    public class DeleteReply : CommandBase
    {
        public Guid Id { get; set; }
        public Guid ForumId { get; set; }
        public Guid TopicId { get; set; }
    }
}
