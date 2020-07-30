using System.Collections.Generic;
using Atlas.Domain.PermissionSets;

namespace Atlas.Models
{
    public class PermissionModel
    {
        public PermissionType Type { get; set; }
        public bool AllUsers { get; set; }
        public IList<RoleModel> Roles { get; set; } = new List<RoleModel>();
    }
}
