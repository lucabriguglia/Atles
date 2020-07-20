using System;

namespace Atlas.Server.Domain
{
    public class Permission
    {
        public Guid PermissionSetId { get; set; }
        public PermissionType Type { get; set; }
        public string RoleId { get; set; }

        public virtual PermissionSet PermissionSet { get; set; }

        public Permission()
        {
            
        }
    }
}
