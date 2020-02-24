using System;

namespace Atlas.Models
{
    public class Permission
    {
        public Guid PermissionSetId { get; set; }
        public PermissionType Type { get; set; }
        public string RoleId { get; set; }
    }
}
