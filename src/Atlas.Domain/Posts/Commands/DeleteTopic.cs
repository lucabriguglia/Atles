using System;

namespace Atlas.Domain.Posts.Commands
{
    public class DeleteTopic : CommandBase
    {
        public Guid Id { get; set; }
        public Guid ForumId { get; set; }
    }
}
