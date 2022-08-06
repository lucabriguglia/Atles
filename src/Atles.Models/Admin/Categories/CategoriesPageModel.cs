namespace Atles.Models.Admin.Categories;

public class CategoriesPageModel
{
    public IList<CategoryModel> Categories { get; set; } = new List<CategoryModel>();

    public class CategoryModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public int TotalForums { get; set; }
        public int TotalTopics { get; set; }
        public int TotalReplies { get; set; }
        public string PermissionSetName { get; set; }
    }
}