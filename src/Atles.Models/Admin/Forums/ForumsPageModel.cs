namespace Atles.Models.Admin.Forums;

public class ForumsPageModel
{
    public IList<CategoryModel> Categories { get; set; } = new List<CategoryModel>();
    public IList<ForumModel> Forums { get; set; } = new List<ForumModel>();

    public class CategoryModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class ForumModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int SortOrder { get; set; }
        public int TotalTopics { get; set; }
        public int TotalReplies { get; set; }
        public string PermissionSetName { get; set; }
    }
}
