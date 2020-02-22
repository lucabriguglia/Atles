using System;

namespace ForumApp.Domain.Replies
{
    public class Reply
    {
        public Guid Id { get; set; }
        public Guid TopicId { get; set; }
        public string Content { get; set; }
    }
}
