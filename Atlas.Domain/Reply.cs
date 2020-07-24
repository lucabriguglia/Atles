using System;
using Atlas.Domain.Topics;

namespace Atlas.Domain
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

        public Reply(Guid forumId, Guid memberId, string content, StatusType status)
        {
            New(Guid.NewGuid(), forumId, memberId, content, status);
        }

        public Reply(Guid id, Guid forumId, Guid memberId, string content, StatusType status)
        {
            New(id, forumId, memberId, content, status);
        }

        private void New(Guid id, Guid topicId, Guid memberId, string content, StatusType status)
        {
            Id = id;
            TopicId = topicId;
            MemberId = memberId;
            Content = content;
            Status = status;
        }

        public void Delete()
        {
            Status = StatusType.Deleted;
        }
    }
}
