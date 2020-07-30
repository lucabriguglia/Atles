using System.Collections.Generic;
using Atlas.Domain.PermissionSets;

namespace Atlas.Models
{
    public class PermissionModel
    {
        public PermissionType Type { get; set; }
        public bool AllUsers { get; set; }
        public bool RegisteredUsers { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
