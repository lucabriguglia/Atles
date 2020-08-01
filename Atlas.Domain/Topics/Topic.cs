using System;
using System.Collections.Generic;
using Atlas.Domain.Forums;
using Atlas.Domain.Members;
using Atlas.Domain.Replies;

namespace Atlas.Domain.Topics
{
    public class Topic
    {
        public Guid Id { get; private set; }
        public Guid ForumId { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }
        public int RepliesCount { get; private set; }
        public StatusType Status { get; private set; }
        public Guid MemberId { get; private set; }
        public DateTime TimeStamp { get; private set; }

        public virtual Forum Forum { get; set; }
        public virtual Member Member { get; set; }

        public virtual ICollection<Reply> Replies { get; set; }

        public Topic()
        {

        }

        public Topic(Guid forumId, Guid memberId, string title, string content, StatusType status)
        {
            New(Guid.NewGuid(), forumId, memberId, title, content, status);
        }

        public Topic(Guid id, Guid forumId, Guid memberId, string title, string content, StatusType status)
        {
            New(id, forumId, memberId, title, content, status);
        }

        private void New(Guid id, Guid forumId, Guid memberId, string title, string content, StatusType status)
        {
            Id = id;
            ForumId = forumId;
            MemberId = memberId;
            Title = title;
            Content = content;
            Status = status;
            TimeStamp = DateTime.UtcNow;
        }

        public void UpdateDetails(string title, string content, StatusType status)
        {
            Title = title;
            Content = content;
            Status = status;
        }

        public void IncreaseRepliesCount()
        {
            RepliesCount += 1;
        }

        public void DecreaseRepliesCount()
        {
            RepliesCount -= 1;

            if (RepliesCount < 0)
            {
                RepliesCount = 0;
            }
        }

        public void Delete()
        {
            Status = StatusType.Deleted;
        }
    }
}
