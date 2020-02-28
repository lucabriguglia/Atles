using System;
using System.Collections.Generic;

namespace Atlas.Models
{
    public class ForumGroup
    {
        public Guid Id { get; set; }
        public Guid SiteId { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public int TopicsCount { get; set; }
        public int RepliesCounts { get; set; }
        public Guid? PermissionSetId { get; set; }

        public virtual Site Site { get; set; }

        public virtual ICollection<Forum> Forums { get; set; }

        public ForumGroup()
        {
            
        }
    }    
}
