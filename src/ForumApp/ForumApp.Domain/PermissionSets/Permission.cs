using System;

namespace ForumApp.Domain.Profiles
{
    public class Permission
    {
        public Guid PermissionSetId { get; set; }
        public PermissionType Type { get; set; }
        public string RoleId { get; set; }
    }
}
