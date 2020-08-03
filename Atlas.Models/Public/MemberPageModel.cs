using System;
using System.Collections.Generic;

namespace Atlas.Models.Public
{
    public class MemberPageModel
    {
        public MemberModel Member { get; set; } = new MemberModel();

        public IList<TopicModel> LastTopics { get; set; } = new List<TopicModel>();

        public class MemberModel
        {
            public Guid Id { get; set; }
            public string DisplayName { get; set; }
            public int TotalTopics { get; set; }
            public int TotalReplies { get; set; }
            public string GravatarHash { get; set; }
        }

        public class TopicModel
        {
            public Guid Id { get; set; }
            public Guid ForumId { get; set; }
            public string Title { get; set; }
            public DateTime TimeStamp { get; set; }
            public int TotalReplies { get; set; }
            public bool CanRead { get; set; }
        }
    }
}
