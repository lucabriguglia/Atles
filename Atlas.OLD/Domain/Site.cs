using System;
using System.Collections.Generic;

namespace Atlas.Domain
{
    public class Site
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Title { get; private set; }

        public virtual ICollection<ForumGroup> ForumGroups { get; set; }

        public Site()
        {
            
        }

        public Site(string name, string title)
        {
            Id = Guid.NewGuid();
            Name = name;
            Title = title;
        }
    }
}
