using Atles.Domain;

namespace Atles.Models.Public
{
    public class ForumPageModel
    {
        public ForumModel Forum { get; set; } = new();

        public PaginatedData<TopicModel> Topics { get; set; } = new();

        public bool CanRead { get; set; }
        public bool CanStart { get; set; }

        public class ForumModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Slug { get; set; }
        }

        public class TopicModel
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string Slug { get; set; }
            public int TotalReplies { get; set; }
            public Guid UserId { get; set; }
            public string UserDisplayName { get; set; }
            public DateTime TimeStamp { get; set; }
            public string GravatarHash { get; set; }
            public DateTime MostRecentTimeStamp { get; set; }
            public Guid MostRecentUserId { get; set; }
            public string MostRecentUserDisplayName { get; set; }
            public bool Pinned { get; set; }
            public bool Locked { get; set; }
            public bool HasAnswer { get; set; }
            public IList<ReactionModel> Reactions { get; set; } = new List<ReactionModel>();
        }

        public class ReactionModel
        {
            public PostReactionType Type { get; set; }
            public int Count { get; set; }
        }
    }
}
