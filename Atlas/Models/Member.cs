using System;
using System.Collections.Generic;

namespace Atlas.Models
{
    public class Member
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public int TopicsCount { get; set; }
        public int RepliesCount { get; set; }

        public virtual ICollection<Topic> Topics { get; set; }
        public virtual ICollection<Reply> Replies { get; set; }

        public Member()
        {
            
        }
    }    
}
