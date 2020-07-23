using System;
using System.Collections.Generic;

namespace Atlas.Shared.Site.Models
{
    public class IndexPageModel
    {
        public IList<ForumGroupModel> ForumGroups { get; set; } = new List<ForumGroupModel>();

        public class ForumGroupModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }

            public IList<ForumModel> Forums { get; set; }
        }

        public class ForumModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public int TotalTopics { get; set; }
            public int TotalReplies { get; set; }
        }
    }
}
