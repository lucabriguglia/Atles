using System.Collections.Generic;
using Atles.Domain.PermissionSets;

namespace Atlas.Models.Public
{
    public class PermissionModel
    {
        public PermissionType Type { get; set; }
        public bool AllUsers { get; set; }
        public bool RegisteredUsers { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
