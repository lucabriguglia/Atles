using System;

namespace Atlify.Domain.Posts.Commands
{
    public class DeleteTopic : CommandBase
    {
        public Guid Id { get; set; }
        public Guid ForumId { get; set; }
    }
}
