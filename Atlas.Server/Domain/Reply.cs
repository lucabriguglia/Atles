using System;

namespace Atlas.Server.Domain
{
    public class Reply
    {
        public Guid Id { get; set; }
        public Guid TopicId { get; set; }
        public string Content { get; set; }
        public StatusType Status { get; private set; }
        public Guid MemberId { get; set; }

        public virtual Topic Topic { get; set; }
        public virtual Member Member { get; set; }

        public Reply()
        {
            
        }

        public void Delete()
        {
            Status = StatusType.Deleted;
        }
    }
}
