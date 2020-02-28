using System;
using System.Collections.Generic;

namespace Atlas.Models
{
    public class Site
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ForumGroup> ForumGroups { get; set; }

        public Site()
        {
            
        }
    }
}
