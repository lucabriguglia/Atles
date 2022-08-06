using Atles.Domain;

namespace Atles.Models.Admin.PermissionSets;

public class PermissionSetFormModel
{
    public PermissionSetModel PermissionSet { get; set; } = new();

    public class PermissionSetModel : SiteFormModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public IList<PermissionModel> Permissions { get; set; } = new List<PermissionModel>();
    }

    public class PermissionModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        //public bool Disabled { get; set; }

        public IList<PermissionTypeModel> PermissionTypes { get; set; } = new List<PermissionTypeModel>();
    }

    public class PermissionTypeModel
    {
        public PermissionType Type { get; set; }
        public bool Selected { get; set; }
        public bool Disabled { get; set; }
    }
}
