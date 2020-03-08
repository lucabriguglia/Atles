using System;
using System.Collections.Generic;

namespace Atlas.Models
{
    public class ForumGroup
    {
        public Guid Id { get; private set; }
        public Guid SiteId { get; private set; }
        public string Name { get; private set; }
        public int SortOrder { get; private set; }
        public int TopicsCount { get; private set; }
        public int RepliesCount { get; private set; }
        public Guid? PermissionSetId { get; private set; }

        public virtual Site Site { get; set; }
        public virtual PermissionSet PermissionSet { get; set; }

        public virtual ICollection<Forum> Forums { get; set; }

        public ForumGroup()
        {

        }

        public ForumGroup(Guid siteId, string name, int sortOrder)
        {
            Id = Guid.NewGuid();
            SiteId = siteId;
            Name = name;
            SortOrder = sortOrder;
        }

        public string PermissionSetName() => PermissionSet?.Name;
    }    
}
