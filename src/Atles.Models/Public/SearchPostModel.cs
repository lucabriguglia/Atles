namespace Atles.Models.Public
{
    public class SearchPostModel
    {
        public Guid Id { get; set; }
        public Guid TopicId { get; set; }
        public bool IsTopic { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Content { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid UserId { get; set; }
        public string UserDisplayName { get; set; }
        public Guid ForumId { get; set; }
        public string ForumName { get; set; }
        public string ForumSlug { get; set; }
    }
}