using System;

namespace ForumApp.Domain.Profiles
{
    public class PermissionItem
    {
        public Guid PermissionId { get; set; }
        public PermissionType Type { get; set; }
        public string RoleId { get; set; }
    }
}
