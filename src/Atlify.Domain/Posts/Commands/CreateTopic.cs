using System;

namespace Atlify.Domain.Posts.Commands
{
    public class CreateTopic : CommandBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ForumId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Content { get; set; }
        public StatusType Status { get; set; }
    }
}
