using System;
using Atlas.Domain.Members;
using Atlas.Domain.Topics;

namespace Atlas.Domain.Replies
{
    public class Reply
    {
        public Guid Id { get; private set; }
        public Guid TopicId { get; private set; }
        public string Content { get; private set; }
        public StatusType Status { get; private set; }
        public Guid MemberId { get; private set; }
        public DateTime TimeStamp { get; private set; }

        public virtual Topic Topic { get; set; }
        public virtual Member Member { get; set; }

        public Reply()
        {
            
        }

        public Reply(Guid topicId, Guid memberId, string content, StatusType status)
        {
            New(Guid.NewGuid(), topicId, memberId, content, status);
        }

        public Reply(Guid id, Guid topicId, Guid memberId, string content, StatusType status)
        {
            New(id, topicId, memberId, content, status);
        }

        private void New(Guid id, Guid topicId, Guid memberId, string content, StatusType status)
        {
            Id = id;
            TopicId = topicId;
            MemberId = memberId;
            Content = content;
            Status = status;
            TimeStamp = DateTime.UtcNow;
        }

        public void UpdateDetails(string content, StatusType status)
        {
            Content = content;
            Status = status;
        }

        public void Delete()
        {
            Status = StatusType.Deleted;
        }
    }
}
