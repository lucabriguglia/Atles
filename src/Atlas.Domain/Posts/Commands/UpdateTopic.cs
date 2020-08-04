using System;

namespace Atlas.Domain.Posts.Commands
{
    public class UpdateTopic : CommandBase
    {
        public Guid Id { get; set; }
        public Guid ForumId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public StatusType Status { get; set; }
    }
}
