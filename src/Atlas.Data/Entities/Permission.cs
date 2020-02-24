using System;

namespace Atlas.Data.Entities
{
    public class Permission
    {
        public Guid PermissionSetId { get; set; }
        public PermissionType Type { get; set; }
        public string RoleId { get; set; }
    }
}
