using System;
using System.Collections.Generic;

namespace Atlas.Models.Admin.Members
{
    public class IndexPageModel
    {
        public IList<MemberModel> Members { get; set; } = new List<MemberModel>();

        public class MemberModel
        {
            public Guid Id { get; set; }
            public string DisplayName { get; set; }
            public string Email { get; set; }
            public int TotalTopics { get; set; }
            public int TotalReplies { get; set; }
        }
    }
}
