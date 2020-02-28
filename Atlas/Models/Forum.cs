using System;
using System.Collections.Generic;

namespace Atlas.Models
{
    public class Forum
    {
        public Guid Id { get; set; }
        public Guid ForumGroupId { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public int TopicsCount { get; set; }
        public int RepliesCount { get; set; }
        public Guid? PermissionSetId { get; set; }

        public virtual ForumGroup ForumGroup { get; set; }

        public virtual ICollection<Topic> Topics { get; set; }

        public Forum()
        {
            
        }
    }
}
