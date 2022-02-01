using System.Collections.Generic;
using Atles.Domain.Models;

namespace Atles.Reporting.Models.Public
{
    public class PermissionModel
    {
        public PermissionType Type { get; set; }
        public bool AllUsers { get; set; }
        public bool RegisteredUsers { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
