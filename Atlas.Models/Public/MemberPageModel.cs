using System;

namespace Atlas.Models.Public
{
    public class MemberPageModel
    {
        public MemberModel Member { get; set; } = new MemberModel();

        public class MemberModel
        {
            public Guid Id { get; set; }
            public string DisplayName { get; set; }
            public int TotalTopics { get; set; }
            public int TotalReplies { get; set; }
        }
    }
}
