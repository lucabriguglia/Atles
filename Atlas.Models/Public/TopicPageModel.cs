using System;
using System.Collections.Generic;

namespace Atlas.Models.Public
{
    public class TopicPageModel
    {
        public ForumModel Forum { get; set; } = new ForumModel();
        public TopicModel Topic { get; set; } = new TopicModel();
        public IList<ReplyModel> Replies { get; set; } = new List<ReplyModel>();

        public class ForumModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class TopicModel
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }
            public Guid MemberId { get; set; }
            public string MemberDisplayName { get; set; }
        }

        public class ReplyModel
        {
            public Guid Id { get; set; }
            public string Content { get; set; }
            public Guid MemberId { get; set; }
            public string MemberDisplayName { get; set; }
        }
    }
}
