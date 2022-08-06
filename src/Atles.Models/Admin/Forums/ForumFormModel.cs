namespace Atles.Models.Admin.Forums;

public class ForumFormModel
{
    public ForumModel Forum { get; set; } = new();
    public IList<CategoryModel> Categories { get; set; } = new List<CategoryModel>();
    public IList<PermissionSetModel> PermissionSets { get; set; } = new List<PermissionSetModel>();

    public class ForumModel : SiteFormModel
    {
        public Guid Id { get; set; } = Guid.Empty;
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }

        public Guid PermissionSetId { get; set; } = Guid.Empty;
    }

    public class CategoryModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class PermissionSetModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}