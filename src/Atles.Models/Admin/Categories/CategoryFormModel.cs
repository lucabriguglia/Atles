namespace Atles.Models.Admin.Categories;

public class CategoryFormModel
{
    public CategoryModel Category { get; set; } = new();
    public IList<PermissionSetModel> PermissionSets { get; set; } = new List<PermissionSetModel>();

    public class CategoryModel : SiteFormModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid PermissionSetId { get; set; }
    }

    public class PermissionSetModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}