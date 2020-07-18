using System;
using System.Collections.Generic;

namespace Atlas.Shared.Models.Admin.ForumGroups
{
    public class IndexModel
    {
        public IList<ForumGroupModel> ForumGroups { get; set; } = new List<ForumGroupModel>();

        public class ForumGroupModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public int SortOrder { get; set; }
            public int TotalTopics { get; set; }
            public int TotalReplies { get; set; }
            public string PermissionSetName { get; set; }
        }
    }
}
