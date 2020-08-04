using System;
using System.Collections.Generic;
using Atlas.Domain.Replies;
using Atlas.Domain.Topics;

namespace Atlas.Domain.Members
{
    public class Member
    {
        public Guid Id { get; private set; }
        public string UserId { get; private set; }
        public string Email { get; private set; }
        public string DisplayName { get; private set; }
        public int TopicsCount { get; private set; }
        public int RepliesCount { get; private set; }
        public StatusType Status { get; private set; }

        public virtual ICollection<Topic> Topics { get; set; }
        public virtual ICollection<Reply> Replies { get; set; }
        public virtual ICollection<Event> Events { get; set; }

        public Member()
        {
            
        }

        public Member(Guid id, string userId, string email, string displayName)
        {
            New(id, userId, email, displayName);
        }

        public Member(string userId, string email, string displayName)
        {
            New(Guid.NewGuid(), userId, email, displayName);
        }

        private void New(Guid id, string userId, string email, string displayName)
        {
            Id = id;
            UserId = userId;
            Email = email;
            DisplayName = displayName;
            Status = StatusType.Active;
        }

        public void UpdateDetails(string displayName)
        {
            DisplayName = displayName;
        }

        public void IncreaseTopicsCount()
        {
            TopicsCount += 1;
        }

        public void IncreaseRepliesCount()
        {
            RepliesCount += 1;
        }

        public void DecreaseTopicsCount()
        {
            TopicsCount -= 1;

            if (TopicsCount < 0)
            {
                TopicsCount = 0;
            }
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
