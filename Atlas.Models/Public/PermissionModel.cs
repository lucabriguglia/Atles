using Atlas.Domain.PermissionSets;

namespace Atlas.Models.Public
{
    public class PermissionModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public PermissionType Type { get; set; }
    }
}
