using System;

namespace Atles.Domain.Models.PermissionSets
{
    public class Permission
    {
        public Guid PermissionSetId { get; private set; }
        public PermissionType Type { get; private set; }
        public string RoleId { get; private set; }

        public virtual PermissionSet PermissionSet { get; set; }

        public Permission()
        {
            
        }

        public Permission(Guid permissionSetId, PermissionType type, string roleId)
        {
            PermissionSetId = permissionSetId;
            Type = type;
            RoleId = roleId;
        }
    }
}
