namespace Atles.Models.Admin.PermissionSets;

public class PermissionSetsPageModel
{
    public IList<PermissionSetModel> PermissionSets { get; set; } = new List<PermissionSetModel>();

    public class PermissionSetModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsInUse { get; set; }
    }
}
