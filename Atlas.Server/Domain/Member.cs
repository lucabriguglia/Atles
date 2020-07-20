using System;
using System.Collections.Generic;

namespace Atlas.Server.Domain
{
    public class Member
    {
        public Guid Id { get; private set; }
        public string UserId { get; private set; }
        public int TopicsCount { get; private set; }
        public int RepliesCount { get; private set; }

        public virtual ICollection<Topic> Topics { get; set; }
        public virtual ICollection<Reply> Replies { get; set; }
        public virtual ICollection<Event> Events { get; set; }

        public Member()
        {
            
        }

        public Member(string userId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
        }
    }
}
