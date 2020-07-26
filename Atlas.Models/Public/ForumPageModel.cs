using System;
using System.Collections.Generic;

namespace Atlas.Models.Public
{
    public class ForumPageModel
    {
        public ForumModel Forum { get; set; } = new ForumModel();
        //public IList<TopicModel> Topics { get; set; } = new List<TopicModel>();

        public PaginatedData<TopicModel> Data { get; set; } = new PaginatedData<TopicModel>();

        public class ForumModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class TopicModel
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public int TotalReplies { get; set; }
            public Guid MemberId { get; set; }
            public string MemberDisplayName { get; set; }
        }
    }
}
