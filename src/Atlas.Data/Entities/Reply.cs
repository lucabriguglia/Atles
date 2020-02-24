using System;

namespace Atlas.Data.Entities
{
    public class Reply
    {
        public Guid Id { get; set; }
        public Guid TopicId { get; set; }
        public string Content { get; set; }
        public Guid MemberId { get; set; }

        public virtual Topic Topic { get; set; }
        public virtual Member Member { get; set; }
    }
}
