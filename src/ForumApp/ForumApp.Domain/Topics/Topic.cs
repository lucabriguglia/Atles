using System;

namespace ForumApp.Domain.Topics
{
    public class Topic
    {
        public Guid Id { get; set; }
        public Guid SectionId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid MemberId { get; set; }
        public int Replies { get; set; }
    }
}
