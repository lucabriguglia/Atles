using System;

namespace Atlas.Models.Public
{
    public class SearchPageModel
    {
        public PaginatedData<PostModel> Posts { get; set; } = new PaginatedData<PostModel>();

        public class ForumModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class PostModel
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public int TotalReplies { get; set; }
            public Guid MemberId { get; set; }
            public string MemberDisplayName { get; set; }
            public DateTime TimeStamp { get; set; }
            public string GravatarHash { get; set; }
            public DateTime MostRecentTimeStamp { get; set; }
            public Guid MostRecentMemberId { get; set; }
            public string MostRecentMemberDisplayName { get; set; }
        }
    }
}
