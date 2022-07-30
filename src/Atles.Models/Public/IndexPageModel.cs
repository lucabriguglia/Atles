namespace Atles.Models.Public
{
    public class IndexPageModel
    {
        public IList<CategoryModel> Categories { get; set; } = new List<CategoryModel>();

        public class CategoryModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }

            public Guid PermissionSetId { get; set; }

            public IList<Guid> ForumIds { get; set; } = new List<Guid>();

            public IList<ForumModel> Forums { get; set; } = new List<ForumModel>();
        }

        public class ForumModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Slug { get; set; }
            public string Description { get; set; }
            public int TotalTopics { get; set; }
            public int TotalReplies { get; set; }
            public Guid? LastTopicId { get; set; }
            public string LastTopicTitle { get; set; }
            public string LastTopicSlug { get; set; }
            public DateTime? LastPostTimeStamp { get; set; }
            public Guid? LastPostUserId { get; set; }
            public string LastPostUserDisplayName { get; set; }
            public bool CanViewTopics { get; set; }

            public bool HasLastPost() => LastTopicId != null;

            public Guid? PermissionSetId { get; set; }
        }
    }
}
