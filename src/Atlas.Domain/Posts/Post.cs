using System;
using System.ComponentModel.DataAnnotations.Schema;
using Atlas.Domain.Forums;
using Atlas.Domain.Members;

namespace Atlas.Domain.Posts
{
    public class Post
    {
        public Guid Id { get; private set; }
        [ForeignKey("Topic")]
        public Guid? TopicId { get; private set; }
        public Guid ForumId { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }
        public int RepliesCount { get; private set; }
        public StatusType Status { get; private set; }
        public Guid MemberId { get; private set; }
        public DateTime TimeStamp { get; private set; }

        [ForeignKey("LastReply")]
        public Guid? LastReplyId { get; private set; }

        public virtual Post Topic { get; set; }
        public virtual Forum Forum { get; set; }
        public virtual Member Member { get; set; }
        public virtual Post LastReply { get; set; }

        public Post()
        {

        }

        public static Post CreateTopic(Guid forumId, Guid memberId, string title, string content, StatusType status)
        {
            return new Post(Guid.NewGuid(), null, forumId, memberId, title, content, status);
        }

        public static Post CreateTopic(Guid id, Guid forumId, Guid memberId, string title, string content, StatusType status)
        {
            return new Post(id, null, forumId, memberId, title, content, status);
        }

        public static Post CreateReply(Guid topicId, Guid forumId, Guid memberId, string content, StatusType status)
        {
            return new Post(Guid.NewGuid(), topicId, forumId, memberId, null, content, status);
        }

        public static Post CreateReply(Guid id, Guid topicId, Guid forumId, Guid memberId, string content, StatusType status)
        {
            return new Post(id, topicId, forumId, memberId, null, content, status);
        }

        private Post(Guid id, Guid? topicId, Guid forumId, Guid memberId, string title, string content, StatusType status)
        {
            Id = id;
            TopicId = topicId;
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

        public void UpdateDetails(string content, StatusType status)
        {
            Content = content;
            Status = status;
        }

        public void UpdateLastReply(Guid lastReplyId)
        {
            if (IsTopic())
            {
                LastReplyId = lastReplyId;
            }
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

        public bool IsTopic()
        {
            return TopicId == null;
        }
    }
}
