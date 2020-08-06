using System;

namespace Atlas.Models.Public
{
    public class SearchPostModel
    {
        public Guid Id { get; set; }
        public Guid TopicId { get; set; }
        public bool IsTopic { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid MemberId { get; set; }
        public string MemberDisplayName { get; set; }
        public Guid ForumId { get; set; }
        public string ForumName { get; set; }
    }
}